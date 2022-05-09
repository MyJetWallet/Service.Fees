using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;
using Service.Fees.Client;
using Service.Fees.Grpc;
using Service.Fees.Grpc.Models;
using Service.Fees.MyNoSql;

namespace Service.Fees.Services
{
    public class FeeProfileService : IFeeProfileService
    {
        private readonly IMyNoSqlServerDataWriter<AssetFeesNoSqlEntity> _assetWriter;
        private readonly IMyNoSqlServerDataWriter<FeeProfilesNoSqlEntity> _profileWriter;

        public FeeProfileService(IMyNoSqlServerDataWriter<AssetFeesNoSqlEntity> assetWriter, IMyNoSqlServerDataWriter<FeeProfilesNoSqlEntity> profileWriter)
        {
            _assetWriter = assetWriter;
            _profileWriter = profileWriter;
        }

        public async Task<ProfilesResponse> GetAllProfiles()
        {
            var groups = await _profileWriter.GetAsync(FeeProfilesNoSqlEntity.GeneratePartitionKey(), FeeProfilesNoSqlEntity.GenerateRowKey());
            return new ProfilesResponse()
            {
                Profiles = groups?.Profiles ?? new List<string>()
            };
        }

        public async Task<OperationResponse> CreateProfile(CreateGroupRequest request)
        {
            try
            {
                var groups = await _profileWriter.GetAsync(FeeProfilesNoSqlEntity.GeneratePartitionKey(),
                    FeeProfilesNoSqlEntity.GenerateRowKey());
                var groupsList = groups?.Profiles ?? new List<string>();
                groupsList.Add(request.ProfileId);

                if (!string.IsNullOrWhiteSpace(request.CloneFromProfileId))
                {
                    var assets =
                        (await _assetWriter.GetAsync(
                            AssetFeesNoSqlEntity.GeneratePartitionKey(request.BrokerId, request.CloneFromProfileId)))
                        .Select(t => t.AssetFees).ToList();

                    foreach (var asset in assets)
                    {
                        asset.ProfileId = request.ProfileId;
                    }

                    await _assetWriter.BulkInsertOrReplaceAsync(assets.Select(AssetFeesNoSqlEntity.Create));
                    
                }

                return new OperationResponse()
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorText = e.Message
                };
            }
        }

        public async Task<OperationResponse> DeleteProfile(DeleteProfileRequest request)
        {
            try
            {
                var groups = await _profileWriter.GetAsync(FeeProfilesNoSqlEntity.GeneratePartitionKey(),
                    FeeProfilesNoSqlEntity.GenerateRowKey());
                var groupsList = groups?.Profiles ?? new List<string>();
                groupsList.Remove(request.ProfileId);

                var assets =
                    (await _assetWriter.GetAsync(
                        AssetFeesNoSqlEntity.GeneratePartitionKey(request.BrokerId, request.ProfileId))).ToList();

                foreach (var asset in assets)
                {
                    await _assetWriter.DeleteAsync(asset.PartitionKey, asset.RowKey);
                }

                return new OperationResponse()
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                return new OperationResponse()
                {
                    IsSuccess = false,
                    ErrorText = e.Message
                };
            }
        }
    }
}