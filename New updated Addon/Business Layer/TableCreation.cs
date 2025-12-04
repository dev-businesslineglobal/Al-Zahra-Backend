using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace GardenAPP
{
    public class TableCreation
    {
        #region TableCreation
        public TableCreation()
        {
            try
            {
                this.Configuration();
                this.OTP();
                this.SystemForms();

                //this.ItemCart();

            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("Table Creation Failed: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
        }

        #region ...UDO and UDT Creation



        public void Configuration()
        {
            try
            {
                this.ConfigurationTable();
                //this.Deatil();

                //AddVerifiedFieldToOCRD(); // <-- Add this


                if (!GlobalVariables.oGFun.UDOExists("CONF"))
                {
                    string[,] FindField = new string[,] { { "DocEntry", "DocEntry" } };
                    GlobalVariables.oGFun.RegisterUDO("CONF", "GardenConfig", SAPbobsCOM.BoUDOObjType.boud_MasterData, FindField, "GR_CONF", "", "");
                    FindField = null;
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
            }
            finally
            {
            }
        }
        public void ConfigurationTable()
        {
            try
            {

                GlobalVariables.oGFun.CreateTable("GR_CONF", "Gardening-Configuration", SAPbobsCOM.BoUTBTableType.bott_MasterData);
                GlobalVariables.oGFun.CreateUserFields("@GR_CONF", "BPSeries", "Customer Series", SAPbobsCOM.BoFieldTypes.db_Numeric,11);
                GlobalVariables.oGFun.CreateUserFields("@GR_CONF", "BPGroup", "Customer Group", SAPbobsCOM.BoFieldTypes.db_Numeric,11);
                GlobalVariables.oGFun.CreateUserFields("@GR_CONF", "SOSeries", "Sales Order Series", SAPbobsCOM.BoFieldTypes.db_Numeric,11);
                GlobalVariables.oGFun.CreateUserFields("@GR_CONF", "DelSeries", "Delivery Series", SAPbobsCOM.BoFieldTypes.db_Numeric,11);
                GlobalVariables.oGFun.CreateUserFields("@GR_CONF", "InvSeries", "Invoice Series", SAPbobsCOM.BoFieldTypes.db_Numeric,11);
                GlobalVariables.oGFun.CreateUserFields("@GR_CONF", "IncSeries", "Incoming Series", SAPbobsCOM.BoFieldTypes.db_Numeric, 11);
                GlobalVariables.oGFun.CreateUserFields("@GR_CONF", "MemoSeries", "A/R Credit Series", SAPbobsCOM.BoFieldTypes.db_Numeric,11);
                GlobalVariables.oGFun.CreateUserFields("@GR_CONF", "DPSeries", "Down Payment Series", SAPbobsCOM.BoFieldTypes.db_Numeric,11);
                GlobalVariables.oGFun.CreateUserFields("@GR_CONF", "WHCode", "Ware House Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@GR_CONF", "CardPay", "Card Payment", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@GR_CONF", "CodPay", "Cash on delivery Payment", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);

                GlobalVariables.oGFun.CreateUserFields("OCRD", "Verified", "User-OTP-Verification", SAPbobsCOM.BoFieldTypes.db_Alpha, 1);
                GlobalVariables.oGFun.CreateUserFields("OCRD", "Password", "Password", SAPbobsCOM.BoFieldTypes.db_Alpha, 100);
                GlobalVariables.oGFun.CreateUserFields("OCRD", "OTPCode", "OTP Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 6);


            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
            }
        }

        public void OTP()
        {
            try
            {
                this.OTPTable();
                //this.Deatil();



                if (!GlobalVariables.oGFun.UDOExists("OTP_T"))
                {
                    string[,] FindField = new string[,] { { "DocEntry", "DocEntry" } };
                    GlobalVariables.oGFun.RegisterUDO("OTP_T", "Gardening OTP", SAPbobsCOM.BoUDOObjType.boud_MasterData, FindField, "OTP_TABLE", "", "");
                    FindField = null;
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
            }
            finally
            {
            }
        }
        public void OTPTable()
        {
            try
            {
                GlobalVariables.oGFun.CreateTable("OTP_TABLE", "Gardening-OTP-Table", SAPbobsCOM.BoUTBTableType.bott_MasterData);
                //GlobalVariables.oGFun.CreateUserFields("@OTP_TABLE", "CardCode", "Card Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);   //Code field is generating automatically so I am assigning cardCode from OCRD to Code
                GlobalVariables.oGFun.CreateUserFields("@OTP_TABLE", "Email", "User Email", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@OTP_TABLE", "OTPCode", "OTP Code", SAPbobsCOM.BoFieldTypes.db_Alpha, 6);
                GlobalVariables.oGFun.CreateUserFields("@OTP_TABLE", "CreatedAt", "Creation Date", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@OTP_TABLE", "ExpireAt", "Expiry Date", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@OTP_TABLE", "IsUsed", "Is OTP Used", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Checkbox);
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
            }
        }

        public void SystemForms()
        {
            try
            {
                GlobalVariables.oGFun.CreateUserFields("ORDR", "Cart", "Cart", SAPbobsCOM.BoFieldTypes.db_Alpha, 1, SAPbobsCOM.BoFldSubTypes.st_Checkbox);
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
            }
        }


        public void ItemCart()
        {
            try
            {
                this.ItemCartHeader();
                this.ItemCartDeatil();

                if (!GlobalVariables.oGFun.UDOExists(""))
                {
                    string[,] FindField = new string[,] { { "DocNum", "DocNum" } };
                    GlobalVariables.oGFun.RegisterUDO("CART", "Item Cart", SAPbobsCOM.BoUDOObjType.boud_Document, FindField, "BL_CART", "", "ART1");
                    FindField = null;
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
            }
            finally
            {
            }
        }
        public void ItemCartHeader()
        {
            try
            {

                GlobalVariables.oGFun.CreateTable("BL_CART", "Header", SAPbobsCOM.BoUTBTableType.bott_Document);
                GlobalVariables.oGFun.CreateUserFields("@BL_CART", "CardCode", "CardCode", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);


            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
            }
        }
        public void ItemCartDeatil()
        {
            try
            {

                GlobalVariables.oGFun.CreateTable("ART1", "Detail", SAPbobsCOM.BoUTBTableType.bott_DocumentLines);
                GlobalVariables.oGFun.CreateUserFields("@ART1", "ItemCode", "ItemCode", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@ART1", "ItemName", "ItemName", SAPbobsCOM.BoFieldTypes.db_Alpha, 50);
                GlobalVariables.oGFun.CreateUserFields("@ART1", "Quantity", "ItemQuantity", SAPbobsCOM.BoFieldTypes.db_Float, 50);
                GlobalVariables.oGFun.CreateUserFields("@ART1", "Price", "ItemPrice", SAPbobsCOM.BoFieldTypes.db_Float, 50);
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
            }
        }






        #endregion


    }

    #endregion
}
