using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IotHubDevice.Models;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Options;

namespace IotHubDevice.Repository
{
    public class IoTDeviceProperties
    {
        private static string iothubConnectionString = "HostName=coure-iot-hub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=7JAt7cZD4eKK/9GcE885kCAteuVScU6pYSK7efTpGns=";
        public static RegistryManager registryManager=RegistryManager.CreateFromConnectionString(iothubConnectionString);

        public static DeviceClient client = null;

        public static string myDeviceConnection = "HostName=coure-iot-hub.azure-devices.net;DeviceId=MyIOTDevice;SharedAccessKey=Z9iYtZjcE0/k3+3GWAYOwT2HStT/TWimz5VAsdevqdQ=";
        public static async Task AddDeviceProperties(string deviceName,DeviceProperties deviceProperties)
        {
            if(string.IsNullOrEmpty(deviceName))
            {
                throw new ArgumentNullException("Kindly enter a valid device name");
            }

            else
            {
                client = DeviceClient.CreateFromConnectionString(myDeviceConnection,Microsoft.Azure.Devices.Client.TransportType.Mqtt);
                TwinCollection twinCollection, connectivity;
                twinCollection= new TwinCollection();
                connectivity= new TwinCollection();
                connectivity["type"] = "cellular";
                
                twinCollection["temperature"] = deviceProperties.temperature;
                
                twinCollection["pressure"] = deviceProperties.pressure;
                twinCollection["accurarcy"] = deviceProperties.accurarcy;
               
                twinCollection["frequency"] = deviceProperties.frequency;
                twinCollection["dateTimeLastAppLaunch"] = deviceProperties.dateTimeLastAppLaunch;
               await client.UpdateReportedPropertiesAsync(twinCollection);
                return;

            }

        }

        public static async Task DesiredProperties(string deviceName)
        {
            client = DeviceClient.CreateFromConnectionString(myDeviceConnection, Microsoft.Azure.Devices.Client.TransportType.Mqtt);
            var device = await registryManager.GetTwinAsync(deviceName);
            TwinCollection twinCollection, telemetryconfig;
            twinCollection = new TwinCollection();
            telemetryconfig = new TwinCollection();
            telemetryconfig["temperature"] = "98F";
            twinCollection["telemetryconfig"] = telemetryconfig;
            device.Properties.Desired["telemetryconfig"] = telemetryconfig;
            await registryManager.UpdateTwinAsync(device.DeviceId,device,device.ETag);
            //return;


        }

        public static async  Task <Twin>GetDeviceProperties(string deviceName)
        {
            var device = await registryManager.GetTwinAsync(deviceName);
            return device;
        }

        public static async Task UpdateDeviceTagProperties(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                throw new ArgumentNullException("Please give valid device name");
            }
            else
            {
                var twin = await registryManager.GetTwinAsync(deviceName);
                var patch =
                    @"{
                       tags:{
                            location:{
                                region:'San Francisko',
                                plant:'IOTPro'
                                }
                            }
                    }";
                client = DeviceClient.CreateFromConnectionString(myDeviceConnection, Microsoft.Azure.Devices.Client.TransportType.Mqtt);
                TwinCollection connectivity;
                connectivity =  new TwinCollection();
                connectivity["type"] = "cellular";
                await registryManager.UpdateTwinAsync(twin.DeviceId, patch, twin.ETag);

            }

        }
    }
}
