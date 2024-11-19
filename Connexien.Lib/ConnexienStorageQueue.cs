using Azure.Storage.Queues;
using System.Configuration;
using System.Threading.Tasks;

namespace Connexien.Lib
{
    public class ConnexienStorageQueue
    {
        public async Task AddMessageAsync(string queue, string message)
        {
            QueueClient queueClient = new QueueClient(ConfigurationManager.AppSettings["Azure:StorageConnectionString"], queue, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });

            queueClient.CreateIfNotExists();
            await queueClient.SendMessageAsync(message);
        }
    }
}
