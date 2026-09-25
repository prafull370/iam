using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using ModelLayer.Models.IAM.DataSyncing;
using RepositoryLayer.IAM.IDataSyncing;

namespace DataLayer.IAM.DataSyncing
{
    internal class DataSyncQueue : IDataSyncQueue
    {
        private readonly Channel<DataSyncJob> _queue =
            Channel.CreateUnbounded<DataSyncJob>();

        public void Enqueue(DataSyncJob job)
        {
            _queue.Writer.TryWrite(job);
        }

        public async Task<DataSyncJob> DequeueAsync(CancellationToken token)
        {
            return await _queue.Reader.ReadAsync(token);
        }
    }

}
