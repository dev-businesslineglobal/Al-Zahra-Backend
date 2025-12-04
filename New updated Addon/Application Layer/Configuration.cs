
using SAPbobsCOM;
using System;
using System.Linq;
namespace GardenAPP
{
    internal class Configuration
    {
        #region Variables
        SAPbouiCOM.Form frmConfiguration, Link_Form;
        SAPbouiCOM.ComboBox cmbCustomerSeries,cmbCustomerGroup, cmbSaleOrder, cmbDelivery, cmbInvoice, cmbMemo, cmbIncoming,cmbDownPayment;
        SAPbouiCOM.EditText whsCode, cardPay, codPay;
        int CustomerSeries,CustomerGroup, SaleOrder, Delivery,Invoice, Memo, Incoming,DownPayment=0;
        SAPbobsCOM.Recordset oRecordSet;

        #endregion

        #region LoadForm
        public void LoadAddedConfiguration()
        {
            try
            {

                GlobalVariables.oGFun.LoadXML(frmConfiguration, GlobalVariables.configurationID, GlobalVariables.confXML);
                frmConfiguration = EventHandler.oApplication.Forms.Item(GlobalVariables.configurationID);
                cmbCustomerSeries = frmConfiguration.Items.Item("3").Specific;
                cmbCustomerGroup = frmConfiguration.Items.Item("4").Specific;
                cmbSaleOrder = frmConfiguration.Items.Item("5").Specific;
                cmbDelivery = frmConfiguration.Items.Item("6").Specific;
                cmbInvoice = frmConfiguration.Items.Item("7").Specific;
                cmbMemo = frmConfiguration.Items.Item("8").Specific;
                cmbIncoming = frmConfiguration.Items.Item("9").Specific;
                cmbDownPayment = frmConfiguration.Items.Item("11").Specific;
                whsCode = frmConfiguration.Items.Item("etWare").Specific;
                cardPay = frmConfiguration.Items.Item("etCardP").Specific;
                codPay = frmConfiguration.Items.Item("etCodP").Specific;
                

                GlobalVariables.oGFun.LoadComboBox(cmbCustomerGroup, $@"SELECT T0.""GroupCode"",T0.""GroupName"" FROM OCRG T0 WHERE T0.""GroupType""='C'");
                GlobalVariables.oGFun.LoadComboBox(cmbCustomerSeries, $@"SELECT T0.""Series"", T0.""SeriesName"" FROM NNM1 T0 WHERE T0.""ObjectCode"" ='2'");
                GlobalVariables.oGFun.LoadComboBox(cmbSaleOrder, $@"SELECT T0.""Series"", T0.""SeriesName"" FROM NNM1 T0 WHERE T0.""ObjectCode"" ='17'");
                GlobalVariables.oGFun.LoadComboBox(cmbDelivery, $@"SELECT T0.""Series"", T0.""SeriesName"" FROM NNM1 T0 WHERE T0.""ObjectCode"" ='15'");
                GlobalVariables.oGFun.LoadComboBox(cmbInvoice, $@"SELECT T0.""Series"", T0.""SeriesName"" FROM NNM1 T0 WHERE T0.""ObjectCode"" ='13'");
                GlobalVariables.oGFun.LoadComboBox(cmbMemo, $@"SELECT T0.""Series"", T0.""SeriesName"" FROM NNM1 T0 WHERE T0.""ObjectCode"" ='14'");
                GlobalVariables.oGFun.LoadComboBox(cmbDownPayment, $@"SELECT T0.""Series"", T0.""SeriesName"" FROM NNM1 T0 WHERE T0.""ObjectCode"" ='203'");
                GlobalVariables.oGFun.LoadComboBox(cmbIncoming, $@"SELECT T0.""Series"", T0.""SeriesName"" FROM NNM1 T0 WHERE T0.""ObjectCode"" ='24'");
             
                InitForm();
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("Load : " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
        }

        public void InitForm()
        {
            try
            {
                frmConfiguration = EventHandler.oApplication.Forms.ActiveForm;
                frmConfiguration.Freeze(true);
                this.LoadConfiguration();
                frmConfiguration.Freeze(false);

            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
                frmConfiguration.Freeze(false);
            }
            finally
            {

            }
        }



        private void LoadConfiguration()
        {
            try
            {
                string query = @"
        SELECT 
            T0.""Code"", 
            T0.""Name"", 
            T0.""U_BPSeries"", 
            T0.""U_BPGroup"", 
            T0.""U_SOSeries"", 
            T0.""U_DelSeries"", 
            T0.""U_InvSeries"", 
            T0.""U_IncSeries"",
            T0.""U_MemoSeries"",
            T0.""U_DPSeries"",
            T0.""U_WHCode"",
            T0.""U_CardPay"",
            T0.""U_CodPay""
        FROM ""@GR_CONF"" T0 
        WHERE T0.""Code"" = 'GR'";

                SAPbobsCOM.Recordset oRecSet = GlobalVariables.oGFun.DoQuery(query);

                if (oRecSet.RecordCount > 0)
                {
                    oRecSet.MoveFirst();

                    CustomerSeries = Convert.ToInt32(oRecSet.Fields.Item("U_BPSeries").Value);
                    CustomerGroup = Convert.ToInt32(oRecSet.Fields.Item("U_BPGroup").Value);
                    SaleOrder = Convert.ToInt32(oRecSet.Fields.Item("U_SOSeries").Value);
                    Delivery = Convert.ToInt32(oRecSet.Fields.Item("U_DelSeries").Value);
                    Invoice = Convert.ToInt32(oRecSet.Fields.Item("U_InvSeries").Value);
                    Incoming = Convert.ToInt32(oRecSet.Fields.Item("U_IncSeries").Value);
                    Memo = Convert.ToInt32(oRecSet.Fields.Item("U_MemoSeries").Value);
                    DownPayment = Convert.ToInt32(oRecSet.Fields.Item("U_DPSeries").Value);
                    whsCode.Value = oRecSet.Fields.Item("U_WHCode").Value.ToString();
                    cardPay.Value = oRecSet.Fields.Item("U_CardPay").Value.ToString();
                    codPay.Value = oRecSet.Fields.Item("U_CodPay").Value.ToString();


                    if (CustomerSeries > 0)
                        this.cmbCustomerSeries.Select(CustomerSeries.ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                    if (CustomerGroup > 0)
                        this.cmbCustomerGroup.Select(CustomerGroup.ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                    if (SaleOrder > 0)
                        this.cmbSaleOrder.Select(SaleOrder.ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                    if (Delivery > 0)
                        this.cmbDelivery.Select(Delivery.ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                    if (Invoice > 0)
                        this.cmbInvoice.Select(Invoice.ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                    if (Incoming > 0)
                        this.cmbIncoming.Select(Incoming.ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                    if (Memo > 0)
                        this.cmbMemo.Select(Memo.ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                    if (DownPayment > 0)
                        this.cmbDownPayment.Select(DownPayment.ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText(
                    $"Error loading Configuration: {ex.Message}",
                    SAPbouiCOM.BoMessageTime.bmt_Short,
                    SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }


        #endregion



        #region Validation
        public bool ValidateAll()
        {
            bool functionReturnValue = true;
            try
            {
               

                return true;
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
                functionReturnValue = false;
            }
            finally
            {
            }
            return functionReturnValue;
        }
        #endregion
        public void ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                switch (pVal.EventType)
                {
                    case SAPbouiCOM.BoEventTypes.et_CLICK:
                        try
                        {
                            switch (pVal.ItemUID)
                            {
                                case "1":
                                    if (pVal.BeforeAction == false)
                                    {
                                        AddOrUpdateConfigUDO();
                                    }
                                    break;


                            }
                        }
                        catch (Exception ex)
                        {
                            EventHandler.oApplication.MessageBox(ex.Message);
                        }
                        finally
                        {
                        }
                        break;
                    case SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST:

                        try

                        {

                            SAPbouiCOM.DataTable oDataTable = null;

                            SAPbouiCOM.ChooseFromListEvent oCFLE = (SAPbouiCOM.ChooseFromListEvent)pVal;


                            // Apply filter before the CFL opens





                            oDataTable = oCFLE.SelectedObjects;

                            if ((oDataTable != null) & pVal.BeforeAction == false)

                            {

                                switch (pVal.ItemUID)

                                {


                                    case "etWare":

                                        //HandleWarehouseCFL(pVal);
                                        string whsCode = oDataTable.GetValue("WhsCode", 0).ToString();

                                        // Set directly into your UserDataSource "udWare"
                                        frmConfiguration.DataSources.UserDataSources.Item("udWare").Value = whsCode;

                                        break;

                                    case "etCardP":
                                        string cardPay = oDataTable.GetValue("AcctCode", 0).ToString();
                                        // Set directly into your UserDataSource "udWare"
                                        frmConfiguration.DataSources.UserDataSources.Item("udCard").Value = cardPay;
                                        break;
                                    case "etCodP":
                                        string codPay = oDataTable.GetValue("AcctCode", 0).ToString();
                                        // Set directly into your UserDataSource "udWare"
                                        frmConfiguration.DataSources.UserDataSources.Item("udCod").Value = codPay;
                                        break;


                                }

                            }

                        }

                        catch (Exception ex)

                        {

                            EventHandler.oApplication.StatusBar.SetText("Error: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                        }

                        finally

                        {

                        }

                        break;

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

   
        public void MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {

                switch (pVal.MenuUID)
                {
                    case "1281":
                        {
                            break;
                        }
                    case "1282":
                        {
                            InitForm();

                        }
                        break;
                    case "1288":
                    case "1289":
                    case "1290":
                    case "1291":
                    case "1304":
                        {
                          
                            break;
                        }

                    case "1292":
                        {
          
                            break;
                        }
                    case "1293":
                        {
                            break;
                        }
                    case "1284":
                        {

                            break;
                        }

                }
            }
            catch (Exception ex)
            {
                frmConfiguration.Freeze(false);
                EventHandler.oApplication.MessageBox(ex.Message);
            }
        }



        public void AddOrUpdateConfigUDO()
        {
            try
            {
                SAPbobsCOM.GeneralService oGeneralService = null;
                SAPbobsCOM.GeneralData oGeneralData = null;
                SAPbobsCOM.GeneralDataParams oGeneralParams = null;
                SAPbobsCOM.CompanyService oCompanyService = null;

                oCompanyService = GlobalVariables.oCompany.GetCompanyService();
                oGeneralService = oCompanyService.GetGeneralService("CONF");
                SAPbobsCOM.Recordset oRecSet = (SAPbobsCOM.Recordset)GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string query = $@"SELECT ""Code"" FROM ""@GR_CONF"" WHERE ""Code"" = 'GR'";
                oRecSet.DoQuery(query);

                if (oRecSet.RecordCount > 0)
                {
                    oGeneralParams = (SAPbobsCOM.GeneralDataParams)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
                    oGeneralParams.SetProperty("Code", "GR");

                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                    oGeneralData.SetProperty("Name","GR");
                    oGeneralData.SetProperty("U_BPSeries", this.cmbCustomerSeries.Selected.Value);
                    oGeneralData.SetProperty("U_BPGroup", this.cmbCustomerGroup.Selected.Value);
                    oGeneralData.SetProperty("U_SOSeries", this.cmbSaleOrder.Selected.Value);
                    oGeneralData.SetProperty("U_DelSeries", this.cmbDelivery.Selected.Value);
                    oGeneralData.SetProperty("U_InvSeries", this.cmbInvoice.Selected.Value);
                    oGeneralData.SetProperty("U_IncSeries", this.cmbIncoming.Selected.Value);
                    oGeneralData.SetProperty("U_MemoSeries", this.cmbMemo.Selected.Value);
                    oGeneralData.SetProperty("U_DPSeries", this.cmbDownPayment.Selected.Value);
                    oGeneralData.SetProperty("U_WHCode", this.whsCode.Value);
                    oGeneralData.SetProperty("U_CardPay", this.cardPay.Value);
                    oGeneralData.SetProperty("U_CodPay", this.cardPay.Value);
                    
           

                    oGeneralService.Update(oGeneralData);

                    EventHandler.oApplication.StatusBar.SetText(
                        $"Configuration updated successfully.",
                        SAPbouiCOM.BoMessageTime.bmt_Short,
                        SAPbouiCOM.BoStatusBarMessageType.smt_Success
                    );
                }
                else
                {
                    oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
                    oGeneralData.SetProperty("Code", "GR");
                    oGeneralData.SetProperty("Name","GR");
                    oGeneralData.SetProperty("U_BPSeries", this.cmbCustomerSeries.Selected.Value);
                    oGeneralData.SetProperty("U_BPGroup", this.cmbCustomerGroup.Selected.Value);
                    oGeneralData.SetProperty("U_SOSeries", this.cmbSaleOrder.Selected.Value);
                    oGeneralData.SetProperty("U_DelSeries", this.cmbDelivery.Selected.Value);
                    oGeneralData.SetProperty("U_InvSeries", this.cmbInvoice.Selected.Value);
                    oGeneralData.SetProperty("U_IncSeries", this.cmbIncoming.Selected.Value);
                    oGeneralData.SetProperty("U_MemoSeries", this.cmbMemo.Selected.Value);
                    oGeneralData.SetProperty("U_DPSeries", this.cmbDownPayment.Selected.Value);
                    oGeneralData.SetProperty("U_WHCode", this.whsCode.Value);
                    oGeneralData.SetProperty("U_CardPay", this.cardPay.Value);
                    oGeneralData.SetProperty("U_CodPay", this.cardPay.Value);

                    oGeneralService.Add(oGeneralData);

                    EventHandler.oApplication.StatusBar.SetText(
                        $"Configuration added successfully.",
                        SAPbouiCOM.BoMessageTime.bmt_Short,
                        SAPbouiCOM.BoStatusBarMessageType.smt_Success
                    );
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText(
                    $"Error in AddOrUpdateConfigUDO: {ex.Message}",
                    SAPbouiCOM.BoMessageTime.bmt_Medium,
                    SAPbouiCOM.BoStatusBarMessageType.smt_Error
                );
            }
        }


        private void HandleWarehouseCFL(SAPbouiCOM.ChooseFromListEvent pVal)
        {
            try
            {
                if (pVal.SelectedObjects == null) return;

                SAPbouiCOM.DataTable dt = pVal.SelectedObjects;

                string whsCode = dt.GetValue("WhsCode", 0).ToString();

                // Set directly into your UserDataSource "udWare"
                frmConfiguration.DataSources.UserDataSources.Item("udWare").Value = whsCode;

                EventHandler.oApplication.StatusBar.SetText(
                    $"Warehouse Selected: {whsCode}",
                    SAPbouiCOM.BoMessageTime.bmt_Short,
                    SAPbouiCOM.BoStatusBarMessageType.smt_Success
                );

                // If you have a form mode, set it to update
                if (frmConfiguration.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE)
                    frmConfiguration.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText(
                    "Warehouse CFL Error: " + ex.Message,
                    SAPbouiCOM.BoMessageTime.bmt_Short,
                    SAPbouiCOM.BoStatusBarMessageType.smt_Warning
                );
            }
        }




    }






}
