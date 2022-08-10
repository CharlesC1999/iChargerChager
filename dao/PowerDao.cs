using backend.util;
using Microsoft.Extensions.Options;
using backend.Sqls.mysql;
using System.Collections;
using MySqlConnector;
using System;
using backend.Models.Power;
using System.Linq;
using backend.ViewModels.Power;
using System.Collections.Generic;

namespace backend.dao
{

    public class PowerDao
    {
        private readonly appSettings _appSettings;
        private MysqlConnect _myqlconn;

        public PowerDao(IOptions<appSettings> appSettings)
        {
            this._appSettings = appSettings.Value;
            this._myqlconn = DBchoose.GetDBConnect(_appSettings, "db");
        }

        public ChargerModel GetQRCodeByKey(string Key)
        {
            string sql = @$"
            SELECT
            b.id,
            b.name,
            b.address,
            d.name as chargersupplier_name,
            e.name as chargerlocation_name,
            e.address as chargerlocation_address,
            ST_AsGeoJSON(e.geom) as chargerlocation_geom,
            c.type as chargergun_type,
            c.type_power as chargergun_type_power,
            c.fee as chargergun_fee,
            c.`name` as chargergun_name
            FROM (
                SELECT
                charger_id,
                chargergun_id
                FROM `ChargerQRCode`
                WHERE `key` = UUID_TO_BIN(@key)
            ) a
            JOIN `Charger` b
            ON a.charger_id = b.id
            JOIN `ChargerGun` c
            ON a.chargergun_id = c.id
            JOIN `ChargerSupplier` d
            ON b.chargersupplier_id = d.id
            JOIN `ChargerLocation` e
            ON b.chargerlocation_id = e.id
            ";
            Hashtable ht = new Hashtable();
            ht.Add("@key", new SQLParameter(Key, MySqlDbType.VarChar));
            ChargerModel Result = _myqlconn.GetDataList<ChargerModel>(sql, ht).FirstOrDefault();
            return Result;
        }

        public OrderModel GetChargerOrderNow(string Account)
        {
            string sql = @$"
            SELECT
            a.id,
			a.account,
			BIN_TO_UUID(a.car_id) as car_id,
            REPLACE(a.charger_id, '\0', '') as charger_id,
            REPLACE(a.chargergun_id, '\0', '') as chargergun_id,
            a.status,
            a.createid,
            a.createat,
            a.updateid,
            a.updateat,
            b.plate as car_plate,
            BIN_TO_UUID(b.vehiclestyle_id) as car_vehiclestyle_id,
            c.brand as car_vehiclestyle_brand,
            c.model as car_vehiclestyle_model,
            d.name as charger_name,
            d.address as charger_address,
            BIN_TO_UUID(d.chargerlocation_id) as chargerlocation_id,
            e.name as chargerlocation_name,
            e.address as chargerlocation_address,
            ST_AsGeoJSON(e.geom) as chargerlocation_geom,
            f.type as chargergun_type,
            f.type_power as chargergun_type_power,
            f.fee as chargergun_fee,
            f.name as chargergun_name,
            f.address as chargergun_address,
            BIN_TO_UUID(g.id) as chargersupplier_id,
            g.name as chargersupplier_name
            FROM (
                SELECT
                id,
                account,
                car_id,
                charger_id,
                chargergun_id,
                status,
                createid,
                createat,
                updateid,
                updateat
                FROM `ChargerOrder`
                WHERE account = @account
                ORDER BY createat DESC
                LIMIT 1
            ) a
            JOIN `Car` b
            ON a.car_id = b.id
            JOIN `CarVehicleStyle` c
            ON b.vehiclestyle_id = c.id
            JOIN `Charger` d
            ON a.charger_id = d.id
            JOIN `ChargerLocation` e
            ON d.chargerlocation_id = e.id
            JOIN `ChargerGun` f
            ON a.chargergun_id = f.id
            JOIN `ChargerSupplier` g
            ON d.chargersupplier_id = g.id
            ";
            Hashtable ht = new Hashtable();
            ht.Add("@account", new SQLParameter(Account, MySqlDbType.VarChar));
            OrderModel Result = _myqlconn.GetDataList<OrderModel>(sql, ht).FirstOrDefault();
            return Result;
        }

        public ChargerInfoModel GetChargerOrderNowInfo(int TransNo, string Account)
        {
            string sql = @$"
            SELECT
            BIN_TO_UUID(id) as id,
            charger_id,
            chargergun_id,
            charge_time,
            charge_current,
            charge_voltage,
            charge_kw,
            current_kw,
            soc,
            `time`
            FROM `ChargerInfo`
            WHERE trans_no = @trans_no
            ORDER BY `time` DESC
            LIMIT 1
            ";
            Hashtable ht = new Hashtable();
            ht.Add("@trans_no", new SQLParameter(TransNo, MySqlDbType.Int32));
            ChargerInfoModel Result = _myqlconn.GetDataList<ChargerInfoModel>(sql, ht).FirstOrDefault();
            return Result;
        }

        public ChargerStatusModel GetChargerOrderNowStatus(int TransNo)
        {
            string sql = @$"
            SELECT
            charger_id,
            chargergun_id,
            trans_no,
            vendor_error_code,
            status,
            `time`
            FROM `ChargerStatus`
            WHERE trans_no = @trans_no
            ORDER BY `time` DESC
            LIMIT 1
            ";
            Hashtable ht = new Hashtable();
            ht.Add("@trans_no", new SQLParameter(TransNo, MySqlDbType.Int32));
            ChargerStatusModel Result = _myqlconn.GetDataList<ChargerStatusModel>(sql, ht).FirstOrDefault();
            return Result;
        }

        public int PostChargerTransaction(string OrderId, string Account)
        {
            string sql = @$"
            INSERT INTO `ChargerTransaction`
            (
                order_id,
                createid,
                createat,
                updateid,
                updateat
            ) VALUES (
                @order_id,
                @account,
                NOW(),
                @account,
                NOW()
            )
            ";
            Hashtable ht = new Hashtable();
            ht.Add("@order_id", new SQLParameter(OrderId, MySqlDbType.VarChar));
            ht.Add("@account", new SQLParameter(Account, MySqlDbType.VarChar));
            int id = _myqlconn.ExecuteReturnId(sql, ht);
            return id;
        }

        public int PostChargerOrder(string CarId, string Key, string Account)
        {
            string sql = @$"
            INSERT INTO `ChargerOrder` (
                account,
                car_id,
                charger_id,
                chargergun_id,
                status,
                createid,
                createat,
                updateid,
                updateat
            ) VALUES (
                @account,
                UUID_TO_BIN(@car_id),
                (
                    SELECT
                    charger_id
                    FROM `ChargerQRCode`
                    WHERE `key` = UUID_TO_BIN(@key)
                    LIMIT 1
                ),
                (
                    SELECT
                    chargergun_id
                    FROM `ChargerQRCode`
                    WHERE `key` = UUID_TO_BIN(@key)
                    LIMIT 1
                ),
                0,
                @account,
                NOW(),
                @account,
                NOW()
            );
            ";
            Hashtable ht = new Hashtable();
            ht.Add("@account", new SQLParameter(Account, MySqlDbType.VarChar));
            ht.Add("@car_id", new SQLParameter(CarId, MySqlDbType.VarChar));
            ht.Add("@key", new SQLParameter(Key, MySqlDbType.VarChar));
            int Id = _myqlconn.ExecuteReturnId(sql, ht);
            return Id;
        }

        public void PostChargerInfo(string Key, int TransNo)
        {
            string sql = @$"
            INSERT INTO `ChargerInfo` (
                charger_id,
                chargergun_id,
                trans_no,
                charge_time,
                charge_current,
                charge_voltage,
                charge_kw,
                current_kw,
                soc,
                time
            ) VALUES (
                (
                    SELECT
                    charger_id
                    FROM `ChargerQRCode`
                    WHERE `key` = UUID_TO_BIN(@key)
                    LIMIT 1
                ),
                (
                    SELECT
                    chargergun_id
                    FROM `ChargerQRCode`
                    WHERE `key` = UUID_TO_BIN(@key)
                    LIMIT 1
                ),
                @trans_no,
                @charge_time,
                @charge_current,
                @charge_voltage,
                @charge_kw,
                @current_kw,
                @soc,
                NOW()
            );
            ";
            Hashtable ht = new Hashtable();
            ht.Add("@trans_no", new SQLParameter(TransNo, MySqlDbType.Int32));
            ht.Add("@key", new SQLParameter(Key, MySqlDbType.VarChar));
            ht.Add("@charge_time", new SQLParameter(10, MySqlDbType.Int32));
            ht.Add("@charge_current", new SQLParameter("0", MySqlDbType.VarChar));
            ht.Add("@charge_voltage", new SQLParameter("0", MySqlDbType.VarChar));
            ht.Add("@charge_kw", new SQLParameter("0", MySqlDbType.VarChar));
            ht.Add("@current_kw", new SQLParameter("0", MySqlDbType.VarChar));
            ht.Add("@soc", new SQLParameter("0", MySqlDbType.VarChar));
            _myqlconn.Execute(sql, ht);
        }

        public void PostChargerOrderFinish(string ChargeId, string ChargerGunId, int TransNo)
        {
            string sql = @$"
            UPDATE `ChargerOrder`
            SET status = 2
            WHERE charger_id = @charger_id AND chargergun_id = @chargergun_id AND id = @transaction_id
            ";
            Hashtable ht = new Hashtable();
            ht.Add("@charger_id", new SQLParameter(ChargeId, MySqlDbType.VarChar));
            ht.Add("@chargergun_id", new SQLParameter(ChargerGunId, MySqlDbType.VarChar));
            ht.Add("@transaction_id", new SQLParameter(TransNo, MySqlDbType.Int32));
            _myqlconn.Execute(sql, ht);
        }

        public void PostChargerOrderFinishInfo(string ChargeId, string ChargerGunId, int TransNo)
        {
            string sql = @$"
            UPDATE `ChargerOrder` SET price = (
                SELECT
                *
                FROM (
                    SELECT ROUND(fee * 100) as fee FROM `ChargerGun` WHERE id = @chargergun_id AND charger_id = @charger_id LIMIT 1
                ) a
            ) WHERE id = @trans_no;
            INSERT INTO `ChargerFinish` (
                charger_id,
                chargergun_id,
                trans_no,
                start_time,
                end_time,
                gun_in_time,
                gun_out_time,
                charge_time,
                `time`,
                charge_kw,
                inerrupt_type
            ) VALUES (
                @charger_id,
                @chargergun_id,
                @trans_no,
                (
                    SELECT createat as start_time FROM `ChargerOrder` WHERE id = 135 LIMIT 1
                ),
                (
                    SELECT DATE_ADD(createat, interval 2 hour) as end_time FROM `ChargerOrder` WHERE id = 135 LIMIT 1
                ),
                (
                    SELECT createat as gun_in_time FROM `ChargerOrder` WHERE id = 135 LIMIT 1
                ),
                (
                    SELECT DATE_ADD(createat, interval 2 hour) as gun_out_time FROM `ChargerOrder` WHERE id = 135 LIMIT 1
                ),
                7200,
				NOW(),
                100,
                'N'
            )
            ";
            Hashtable ht = new Hashtable();
            ht.Add("@charger_id", new SQLParameter(ChargeId, MySqlDbType.VarChar));
            ht.Add("@chargergun_id", new SQLParameter(ChargerGunId, MySqlDbType.VarChar));
            ht.Add("@trans_no", new SQLParameter(TransNo, MySqlDbType.Int32));
            _myqlconn.Execute(sql, ht);
        }

        public ChargerPostModel GetChargerPostByKey(string Key, string Account)
        {
            string sql = @$"
            SELECT
            charger_id as station_id,
            chargergun_id as charger_id,
            (
                SELECT id as trans_no FROM `ChargerOrder` WHERE account = @account AND status = 0 LIMIT 1
            ) as trans_no
            FROM `ChargerQRCode`
            WHERE `key` = UUID_TO_BIN(@key)
            ";
            Hashtable ht = new Hashtable();
            ht.Add("@account", new SQLParameter(Account, MySqlDbType.VarChar));
            ht.Add("@key", new SQLParameter(Key, MySqlDbType.VarChar));
            ChargerPostModel Result = _myqlconn.GetDataList<ChargerPostModel>(sql, ht).FirstOrDefault();
            return Result;
        }

        public void CancelChargerOrder(string Account)
        {
            string sql = @$"
            UPDATE `ChargerOrder`
            SET status = -1
            WHERE account = @account AND status = 0
            ";
            Hashtable ht = new Hashtable();
            ht.Add("@account", new SQLParameter(Account, MySqlDbType.VarChar));
            _myqlconn.Execute(sql, ht);
        }

        public void UpdateChargerOrderStatus(int OrderId, int Status, string Account)
        {
            string sql = @$"
            UPDATE `ChargerOrder`
            SET status = @status, updateid = @account, updateat = NOW()
            WHERE id = @order_id
            ";
            Hashtable ht = new Hashtable();
            ht.Add("@order_id", new SQLParameter(OrderId, MySqlDbType.Int16));
            ht.Add("@status", new SQLParameter(Status, MySqlDbType.Int16));
            ht.Add("@account", new SQLParameter(Account, MySqlDbType.VarChar));
            _myqlconn.Execute(sql, ht);
        }
    }
}