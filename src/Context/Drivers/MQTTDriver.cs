using Context.DAL.Data;
using Context.DAL.Data.DataPoints;
using Context.DAL.Data.Sources;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Context.Drivers
{
    public class MQTTDriver : Driver
    {
        IManagedMqttClient Client;
        IMqttClientOptions Options;

        MQTTDatasource DataSource;

        public MQTTDriver(MQTTDatasource src) : base(src.Name)
        {
            DataSource = src;
        }

        public override async Task Connect()
        {
            Client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnSubscriberConnected);
            Client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnSubscriberDisconnected);
            Client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnMessageReceived);

            await Client.StartAsync(new ManagedMqttClientOptions { ClientOptions = Options });

            List<MqttTopicFilter> topicFilters = new List<MqttTopicFilter>();

            foreach(DataPoint dps in this.DataSource.DataPoints)
            {
                if(dps.GetType() == typeof(MQTTDataPoint))
                {
                    MQTTDataPoint mydatapoint = dps as MQTTDataPoint;

                    MqttTopicFilter topicFilter = new MqttTopicFilter
                    {
                        Topic = mydatapoint.TopicName,
                        QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce
                    };

                    topicFilters.Add(topicFilter);

                    AddDataPoint(mydatapoint.TopicName, mydatapoint);
                }
            }

            await Client.SubscribeAsync(topicFilters);
        }

        private void OnMessageReceived(MqttApplicationMessageReceivedEventArgs obj)
        {
            String message = obj.ApplicationMessage.ConvertPayloadToString();

            MQTTItem mqtttem = JsonConvert.DeserializeObject<MQTTItem>(message);

            if(mqtttem != null)
            {
                Measurement measurement = new Measurement();

                DataPoint datapoint = GetDataPoint(obj.ApplicationMessage.Topic);

                if(datapoint != null)
                {
                    measurement.DataPointObject = datapoint;
                    measurement.Time = Utilities.Converter.ConvertUnixTimeStampToDate(mqtttem.TimeStamp);
                    if (datapoint.DataType == DataType.Boolean)
                    {
                        measurement.Value = Utilities.Converter.ConvertToBoolean(mqtttem.Value);
                        AddBinaryMeasurement(datapoint.DatabaseName, measurement);
                    }
                    else
                    {
                        if(datapoint.Offset > 0)
                        {
                            measurement.Value = (float)(Utilities.Converter.ConvertToDouble(mqtttem.Value) / datapoint.Offset);
                        }
                        else
                        {
                            measurement.Value = mqtttem.Value;
                        }

                        AddNumericMeasurement(datapoint.DatabaseName, measurement);
                    }

                    
                }
                //measurement.Value 
            }

        }

        private void OnSubscriberDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            IsConnected = false;
        }

        private void OnSubscriberConnected(MqttClientConnectedEventArgs obj)
        {
            IsConnected = true;
        }

        public async override Task Create()
        {
            String ClientID = Guid.NewGuid().ToString();

            String FinalUrl = DataSource.Host + ":" + DataSource.Port;

            Options = new MqttClientOptionsBuilder().
                WithClientId(ClientID).
                WithTcpServer(DataSource.Host, DataSource.Port).
                WithCleanSession().
                Build();

            Client = new MqttFactory().CreateManagedMqttClient();
        }

        public async override Task Disconnect()
        {
           // throw new NotImplementedException();
        }

        public async override Task Read()
        {
           // throw new NotImplementedException();
        }
    }

    public class MQTTItem
    {
        public String Namen { get; set; }
        public long TimeStamp { get; set; }
        public Object Value { get; set; }
    }
}
