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
    public class FeeGroupService : IFeeGroupService
    {
        private readonly IMyNoSqlServerDataWriter<SpotInstrumentFeesNoSqlEntity> _spotInstrumentWriter;
        private readonly IMyNoSqlServerDataWriter<AssetFeesNoSqlEntity> _assetWriter;
        private readonly IMyNoSqlServerDataWriter<GroupsNoSqlEntity> _groupWriter;

        public FeeGroupService(IMyNoSqlServerDataWriter<SpotInstrumentFeesNoSqlEntity> spotInstrumentWriter, IMyNoSqlServerDataWriter<AssetFeesNoSqlEntity> assetWriter, IMyNoSqlServerDataWriter<GroupsNoSqlEntity> groupWriter)
        {
            _spotInstrumentWriter = spotInstrumentWriter;
            _assetWriter = assetWriter;
            _groupWriter = groupWriter;
        }

        public async Task<GroupsResponse> GetAllGroups()
        {
            var groups = await _groupWriter.GetAsync(GroupsNoSqlEntity.GeneratePartitionKey(), GroupsNoSqlEntity.GenerateRowKey());
            return new GroupsResponse()
            {
                Groups = groups?.Groups ?? new List<string>()
            };
        }

        public async Task<OperationResposne> CreateGroup(CreateGroupRequest request)
        {
            try
            {
                var groups = await _groupWriter.GetAsync(GroupsNoSqlEntity.GeneratePartitionKey(),
                    GroupsNoSqlEntity.GenerateRowKey());
                var groupsList = groups?.Groups ?? new List<string>();
                groupsList.Add(request.GroupId);

                if (!string.IsNullOrWhiteSpace(request.CloneFromGroupId))
                {
                    var assets =
                        (await _assetWriter.GetAsync(
                            AssetFeesNoSqlEntity.GeneratePartitionKey(request.BrokerId, request.CloneFromGroupId)))
                        .Select(t => t.AssetFees).ToList();

                    foreach (var asset in assets)
                    {
                        asset.GroupId = request.GroupId;
                    }

                    await _assetWriter.BulkInsertOrReplaceAsync(assets.Select(AssetFeesNoSqlEntity.Create));
                    
                    var instruments =
                        (await _spotInstrumentWriter.GetAsync(
                            SpotInstrumentFeesNoSqlEntity.GeneratePartitionKey(request.BrokerId, request.CloneFromGroupId)))
                        .Select(t => t.SpotInstrumentFees).ToList();

                    foreach (var instrument in instruments)
                    {
                        instrument.GroupId = request.GroupId;
                    }

                    await _spotInstrumentWriter.BulkInsertOrReplaceAsync(instruments.Select(SpotInstrumentFeesNoSqlEntity.Create));
                }

                return new OperationResposne()
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                return new OperationResposne()
                {
                    IsSuccess = false,
                    ErrorText = e.Message
                };
            }
        }

        public async Task<OperationResposne> DeleteGroup(DeleteGroupRequest request)
        {

            
            try
            {
                var groups = await _groupWriter.GetAsync(GroupsNoSqlEntity.GeneratePartitionKey(),
                    GroupsNoSqlEntity.GenerateRowKey());
                var groupsList = groups?.Groups ?? new List<string>();
                groupsList.Remove(request.GroupId);
                

                var assets =
                    (await _assetWriter.GetAsync(
                        AssetFeesNoSqlEntity.GeneratePartitionKey(request.BrokerId, request.GroupId))).ToList();

                foreach (var asset in assets)
                {
                    await _assetWriter.DeleteAsync(asset.PartitionKey, asset.RowKey);
                }

                
                var instruments =
                    (await _spotInstrumentWriter.GetAsync(
                        SpotInstrumentFeesNoSqlEntity.GeneratePartitionKey(request.BrokerId, request.GroupId))).ToList();

                foreach (var instrument in instruments)
                {
                    await _spotInstrumentWriter.DeleteAsync(instrument.PartitionKey, instrument.RowKey);
                }

                return new OperationResposne()
                {
                    IsSuccess = true
                };
            }
            catch (Exception e)
            {
                return new OperationResposne()
                {
                    IsSuccess = false,
                    ErrorText = e.Message
                };
            }
        }
    }
}