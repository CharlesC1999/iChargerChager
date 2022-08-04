using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using backend.dao;
using backend.Models;
using backend.Models.Power;
using backend.util;
using backend.ViewModels.Power;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace backend.Services
{
    public class PowerService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly PowerDao _PowerDao;
        private readonly appSettings _appSettings;
        public PowerService(IHttpClientFactory clientFactory, PowerDao PowerDao, IOptions<appSettings> appSettings)
        {
            this._PowerDao = PowerDao;
            this._appSettings = appSettings.Value;
            this._clientFactory = clientFactory;
        }

        public ChargerViewModel GetQRCodeByKey(string Key)
        {
            ChargerViewModel Result = new ChargerViewModel();
            ChargerModel Data = _PowerDao.GetQRCodeByKey(Key);
            Result = new ChargerViewModel
            {
                id = Data.id,
                name = Data.name,
                address = Data.address,
                chargersupplier_name = Data.chargersupplier_name,
                chargerlocation_name = Data.chargerlocation_name,
                chargerlocation_address = Data.chargerlocation_address,
                chargerlocation_geom = JsonConvert.DeserializeObject<GeometryModel>(Data.chargerlocation_geom),
                chargergun_type = Data.chargergun_type,
                chargergun_type_power = Data.chargergun_type_power,
                chargergun_fee = Data.chargergun_fee,
                chargergun_name = Data.chargergun_name,
            };
            return Result;
        }

        public OrderViewModel GetChargerOrderNow(string Account)
        {
            OrderViewModel Result = new OrderViewModel();
            OrderModel Data = _PowerDao.GetChargerOrderNow(Account);
            Result = new OrderViewModel()
            {
                id = Data.id,
                account = Data.account,
                car_id = Data.car_id,
                car = new CarViewModel()
                {
                    id = Data.car_id,
                    plate = Data.car_plate,
                    vehiclestyle_id = Data.car_vehiclestyle_id,
                    vehiclestyle = new CarVehicleStyleViewModel()
                    {
                        id = Data.car_vehiclestyle_id,
                        brand = Data.car_vehiclestyle_brand,
                        model = Data.car_vehiclestyle_model,
                    }
                },
                charger_id = Data.charger_id,
                charger = new ChargerViewModel
                {
                    id = Data.charger_id,
                    name = Data.charger_name,
                    address = Data.charger_address,
                    chargersupplier_name = Data.chargersupplier_name,
                    chargerlocation_name = Data.chargerlocation_name,
                    chargerlocation_address = Data.chargerlocation_address,
                    chargerlocation_geom = JsonConvert.DeserializeObject<GeometryModel>(Data.chargerlocation_geom),
                    chargergun_type = Data.chargergun_type,
                    chargergun_type_power = Data.chargergun_type_power,
                    chargergun_fee = Data.chargergun_fee,
                    chargergun_name = Data.chargergun_name,
                },
                status = Data.status,
                createid = Data.createid,
                createat = Data.createat,
                updateid = Data.updateid,
                updateat = Data.updateat,
            };
            return Result;
        }

        public OrderListViewModel GetChargerOrder(int Page, int PageCount, bool Order, string Account)
        {
            OrderListViewModel Result = new OrderListViewModel();
            List<OrderModel> DataList = _PowerDao.GetChargerOrder(Page, PageCount, Order, Account);

            var pageinfo = _PowerDao.GetChargerOrderCount(Account);
            pageinfo.now_page = Page;
            var pagemax = (int)Math.Ceiling(pageinfo.page_count / (decimal)PageCount);
            pageinfo.max_page = pagemax == 0 ? 1 : pagemax;

            Result.list = DataList.Select(Data => new OrderViewModel()
            {
                id = Data.id,
                account = Data.account,
                car_id = Data.car_id,
                car = new CarViewModel()
                {
                    id = Data.car_id,
                    plate = Data.car_plate,
                    vehiclestyle_id = Data.car_vehiclestyle_id,
                    vehiclestyle = new CarVehicleStyleViewModel()
                    {
                        id = Data.car_vehiclestyle_id,
                        brand = Data.car_vehiclestyle_brand,
                        model = Data.car_vehiclestyle_model,
                    }
                },
                charger_id = Data.charger_id,
                charger = new ChargerViewModel
                {
                    id = Data.charger_id,
                    name = Data.charger_name,
                    address = Data.charger_address,
                    chargersupplier_name = Data.chargersupplier_name,
                    chargerlocation_name = Data.chargerlocation_name,
                    chargerlocation_address = Data.chargerlocation_address,
                    chargerlocation_geom = JsonConvert.DeserializeObject<GeometryModel>(Data.chargerlocation_geom),
                    chargergun_type = Data.chargergun_type,
                    chargergun_type_power = Data.chargergun_type_power,
                    chargergun_fee = Data.chargergun_fee,
                    chargergun_name = Data.chargergun_name,
                },
                status = Data.status,
                createid = Data.createid,
                createat = Data.createat,
                updateid = Data.updateid,
                updateat = Data.updateat,
            }).ToList();
            Result.pageinfo = new PageViewModel
            {
                page_count = pageinfo.page_count,
                now_page = pageinfo.now_page,
                max_page = pageinfo.max_page
            };
            return Result;
        }

        public ChargerInfoViewModel GetChargerOrderNowInfo(int TransNo, string Account)
        {
            ChargerInfoViewModel Result = new ChargerInfoViewModel();
            ChargerInfoModel Data = _PowerDao.GetChargerOrderNowInfo(TransNo, Account);
            Result = Data is null ? null : new ChargerInfoViewModel()
            {
                id = Data.id,
                charger_id = Data.charger_id,
                chargergun_id = Data.chargergun_id,
                charge_time = Data.charge_time,
                charge_current = Data.charge_current,
                charge_kw = Data.charge_kw,
                current_kw = Data.current_kw,
                soc = Data.soc,
                time = Data.time,
            };
            return Result;
        }

        public async Task<int> PostChargerOrder(PowerPostModel model, string Account)
        {
            // 新增訂單
            int OrderId = _PowerDao.PostChargerOrder(
                model.CarId,
                model.Key,
                Account
            );
            _PowerDao.PostChargerInfo(
                model.Key,
                OrderId
            );

            // await PostChargerStart(model.Key, Account);

            return OrderId;
        }

        public bool PostChargerOrderFinish(PowerFinishPostModel model)
        {
            // 新增訂單
            _PowerDao.PostChargerOrderFinish(
                model.ChargerId,
                model.ChargerGunId,
                model.TransNo
            );
            return true;
        }

        public async Task PostChargerEnd(ChargerPostModel Data)
        {
            if (Data is null) throw new Exception("無法結束充電槍");
            if (Data.trans_no == 0) throw new Exception("無法結束充電槍");

            var url = "https://gochabar.japaneast.cloudapp.azure.com/etgapi/api/ev_stop";
            var client = _clientFactory.CreateClient();
            string authValue = "HDREAPIKEY";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authValue);
            var body = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(Data),
                Encoding.UTF8,
                Application.Json);
            var response = await client.PostAsync(url, body);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception("無法啟動充電槍");
            }
        }

        public async Task PostChargerStart(string Key, string Account)
        {
            ChargerPostModel Data = _PowerDao.GetChargerPostByKey(Key, Account);
            if (Data is null) throw new Exception("無法啟動充電槍");
            if (Data.trans_no == 0) throw new Exception("無法啟動充電槍");

            var url = "https://gochabar.japaneast.cloudapp.azure.com/etgapi/api/ev_start";
            var client = _clientFactory.CreateClient();
            string authValue = "HDREAPIKEY";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authValue);
            var body = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(Data),
                Encoding.UTF8,
                Application.Json);
            var response = await client.PostAsync(url, body);
            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new Exception("無法啟動充電槍");
            }
        }

        public void CancelChargerOrder(string Account)
        {
            _PowerDao.CancelChargerOrder(Account);
        }
    }
}