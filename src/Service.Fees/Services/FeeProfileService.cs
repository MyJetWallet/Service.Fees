using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyNoSqlServer.Abstractions;
using Service.Fees.Client;
using Service.Fees.Domain.Models;
using Service.Fees.Grpc;
using Service.Fees.Grpc.Models;
using Service.Fees.MyNoSql;

namespace Service.Fees.Services
{
    public class FeeProfileService : IFeeProfileService
    {
        private readonly IMyNoSqlServerDataWriter<AssetFeesNoSqlEntity> _assetWriter;
        private readonly IMyNoSqlServerDataWriter<DepositFeesNoSqlEntity> _depositWriter;
        private readonly IMyNoSqlServerDataWriter<FeeProfilesNoSqlEntity> _profileWriter;

        public FeeProfileService(IMyNoSqlServerDataWriter<AssetFeesNoSqlEntity> assetWriter, IMyNoSqlServerDataWriter<FeeProfilesNoSqlEntity> profileWriter, IMyNoSqlServerDataWriter<DepositFeesNoSqlEntity> depositWriter)
        {
            _assetWriter = assetWriter;
            _profileWriter = profileWriter;
            _depositWriter = depositWriter;
        }

        public async Task<ProfilesResponse> GetAllProfiles()
        {
            var groups = await _profileWriter.GetAsync(FeeProfilesNoSqlEntity.GeneratePartitionKey(), FeeProfilesNoSqlEntity.GenerateRowKey());
            
            if (groups.DepositProfiles == null || !groups.DepositProfiles.Any())
            {
                var list = new List<string>() {FeeProfileConsts.DefaultProfile};
                await _profileWriter.InsertOrReplaceAsync(FeeProfilesNoSqlEntity.Create(groups.Profiles, list));
            }
            
            return new ProfilesResponse()
            {
                WithdrawalProfiles = groups?.Profiles ?? new List<string>(),
                DepositProfiles = groups?.DepositProfiles ?? new List<string>() { FeeProfileConsts.DefaultProfile }
            };
        }

        public async Task<OperationResponse> CreateWithdrawalProfile(CreateGroupRequest request)
        {
            try
            {
                var groups = await _profileWriter.GetAsync(FeeProfilesNoSqlEntity.GeneratePartitionKey(),
                    FeeProfilesNoSqlEntity.GenerateRowKey());
                var withdrawalList = groups?.Profiles ?? new List<string>();
                var depositList = groups?.DepositProfiles ?? new List<string>() {FeeProfileConsts.DefaultProfile};
                withdrawalList.Add(request.ProfileId);
                await _profileWriter.InsertOrReplaceAsync(FeeProfilesNoSqlEntity.Create(withdrawalList.Distinct().ToList(), depositList));
                
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

                    await _assetWriter.BulkInsertOrReplaceAsync(assets.Select(AssetFeesNoSqlEntity.Create).ToList());
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

        public async Task<OperationResponse> DeleteWithdrawalProfile(DeleteProfileRequest request)
        {
            try
            {
                var groups = await _profileWriter.GetAsync(FeeProfilesNoSqlEntity.GeneratePartitionKey(),
                    FeeProfilesNoSqlEntity.GenerateRowKey());
                var withdrawalList = groups?.Profiles ?? new List<string>();
                var depositList = groups?.DepositProfiles ?? new List<string>() {FeeProfileConsts.DefaultProfile};

                var list = withdrawalList.Distinct().ToList();
                list.Remove(request.ProfileId);
                await _profileWriter.InsertOrReplaceAsync(FeeProfilesNoSqlEntity.Create(list, depositList));

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
        
        public async Task<OperationResponse> CreateDepositProfile(CreateGroupRequest request)
        {
            try
            {
                var groups = await _profileWriter.GetAsync(FeeProfilesNoSqlEntity.GeneratePartitionKey(),
                    FeeProfilesNoSqlEntity.GenerateRowKey());
                var withdrawalList = groups?.Profiles ?? new List<string>() {FeeProfileConsts.DefaultProfile};
                var depositList = groups?.DepositProfiles ?? new List<string>();
                depositList.Add(request.ProfileId);
                await _profileWriter.InsertOrReplaceAsync(FeeProfilesNoSqlEntity.Create(withdrawalList, depositList.Distinct().ToList()));
                
                if (!string.IsNullOrWhiteSpace(request.CloneFromProfileId))
                {
                    var assets =
                        (await _depositWriter.GetAsync(
                            DepositFeesNoSqlEntity.GeneratePartitionKey(request.BrokerId, request.CloneFromProfileId)))
                        .Select(t => t.DepositFees).ToList();

                    foreach (var asset in assets)
                    {
                        asset.ProfileId = request.ProfileId;
                    }

                    await _depositWriter.BulkInsertOrReplaceAsync(assets.Select(DepositFeesNoSqlEntity.Create).ToList());
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

        public async Task<OperationResponse> DeleteDepositProfile(DeleteProfileRequest request)
        {
            try
            {
                var groups = await _profileWriter.GetAsync(FeeProfilesNoSqlEntity.GeneratePartitionKey(),
                    FeeProfilesNoSqlEntity.GenerateRowKey());
                var withdrawalList = groups?.Profiles ?? new List<string>() {FeeProfileConsts.DefaultProfile};
                var depositList = groups?.DepositProfiles ?? new List<string>() ;

                var list = depositList.Distinct().ToList();
                list.Remove(request.ProfileId);
                await _profileWriter.InsertOrReplaceAsync(FeeProfilesNoSqlEntity.Create(withdrawalList, list));

                var assets =
                    (await _depositWriter.GetAsync(
                        DepositFeesNoSqlEntity.GeneratePartitionKey(request.BrokerId, request.ProfileId))).ToList();

                foreach (var asset in assets)
                {
                    await _depositWriter.DeleteAsync(asset.PartitionKey, asset.RowKey);
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