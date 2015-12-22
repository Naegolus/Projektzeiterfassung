<%--    Copyright © 2015 by Commend International GmbH. All rights reserved.

    This program is free software: you can redistribute it and/or  modify
    it under the terms of the GNU Affero General Public License, version 3,
    as published by the Free Software Foundation.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.--%>

<%@ Page Title="TTImport" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" Inherits="com.commend.tools.PZE.TtImport" Async="true" Codebehind="TTImport.aspx.cs" 
    ValidateRequest="false"%>

<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Xpo.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Xpo" TagPrefix="dx" %>


<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:ScriptManager ID="ScripManager1" runat="server" EnablePartialRendering="true" />
    <script language="javascript" type="text/jscript">

        function OnProjectChanged(sender) {
            cmbDefaultPSPCode.PerformCallback(cmbDefaultProject.GetValue().toString());
            cmbDefaultActivity.PerformCallback(sender.GetValue().toString());
        }

        function OnCmbDefaultActivityDropDown(sender) {
            if (cmbDefaultProject.GetValue() != null) {
                sender.PerformCallback(cmbDefaultProject.GetValue().toString());
            }
        }

        function OnCmbActivityDropDown(sender) {
            //if (cmbActivity.GetValue() != null) {
                sender.PerformCallback(cmbDefaultProject.GetValue().toString());
            //}
        }

    </script>
    <fieldset>
        <legend>TestTrack import</legend>
        <div>

        <dx:ASPxButton style="float:left; margin-right: 3px; margin-bottom: 3px;" ID="FetchTTDataTodayButton" runat="server" 
                OnClick="FetchTtDataButtonClick" Text="heute" />
            
        <dx:ASPxButton Enabled="true" style="float:left; margin-right: 3px; margin-bottom: 3px;" ID="FetchTTDataYesterdayButton" runat="server" 
                OnClick="FetchTtDataButtonClick" Text="gestern" />
            
        <dx:ASPxButton Enabled="true" style="float:left; margin-right: 3px; margin-bottom: 3px;" ID="FetchTTDataThisWeekButton" runat="server" 
                OnClick="FetchTtDataButtonClick" Text="diese Woche" />

        <dx:ASPxButton Enabled="true" style="float:left; margin-right: 3px; margin-bottom: 3px;" ID="FetchTTDataLastWeekButton" runat="server" 
                OnClick="FetchTtDataButtonClick" Text="letzte Woche" />
            
        <dx:ASPxButton Enabled="true" style="float:left; margin-right: 3px; margin-bottom: 3px;" ID="FetchTTDataThisMonthButton" runat="server" 
                OnClick="FetchTtDataButtonClick" Text="diesen Monat" />
        
        <dx:ASPxButton Enabled="true" style="float:left; margin-right: 3px; margin-bottom: 3px;" ID="FetchTTDataLastMonthButton" runat="server" 
                OnClick="FetchTtDataButtonClick" Text="letzten Monat" />
        <p>
            Einträge von  <dx:ASPxLabel ID="lbDateFrom" runat="server" Text="01.01.2001"/>
            bis <dx:ASPxLabel ID="lbDateTo" runat="server" Text="01.01.2001"/>.
        </p>   
        </div>
        <div>
            <table>
                <tr>
                    <td>
                        <dx:ASPxLabel style="float:right; margin-right: 3px" runat="server" Text ="Projekt"/>
                    </td>
                    <td>
                        <dx:ASPxComboBox runat="server" ID="CmbDefaultProject" ClientInstanceName="cmbDefaultProject"
                            DataSourceID="ProjectsXpoData" IncrementalFilteringMode="Contains" ValueField="Oid" ValueType="System.Int32" 
                            Width="300px" OnValueChanged="OnCmbDefaultProject_ValueChanged"
                            style="float:left; margin-right: 3px; margin-bottom: 3px;">
                            <ClientSideEvents SelectedIndexChanged="function(s, e) { OnProjectChanged(s); }"/>
                            <Columns>
                                <dx:ListBoxColumn FieldName="ProjectNumber" Caption="Nr." Width="20%" />
                                <dx:ListBoxColumn FieldName="ProjectName" Caption="Bezeichnung"  Width="80%" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </td>
                </tr>
                <tr>
                    <td>
                         <dx:ASPxLabel style="float:right; margin-right: 3px" runat="server" Text ="PSP Code"/>
                    </td>
                    <td>
                        <dx:ASPxComboBox ID="CmbDefaultPSPCode" ClientInstanceName="cmbDefaultPSPCode" runat="server" DataSourceID="PspCodeXpoData"
                            ValueField="Oid" ValueType="System.Int32" EnableSynchronization="False"
                            Width="300px" OnValueChanged="OnCmbDefaultPspCode_ValueChanged" OnCallback="OnCmbDefaultPSPCode_Callback"
                            style="float:left; margin-right: 3px; margin-bottom: 3px;" IncrementalFilteringMode="Contains">
                            <Columns>
                                <dx:ListBoxColumn FieldName="PspCodeNumber" Caption="Code" Width="20%" />
                                <dx:ListBoxColumn FieldName="PspCodeName" Caption="Bezeichnung"  Width="80%" />
                            </Columns>
                        </dx:ASPxComboBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <dx:ASPxLabel style="float:right; margin-right: 3px" runat="server" Text="Standard-Tätigkeit"/>
                    </td>
                    <td>
                        <dx:ASPxComboBox ID="CmbDefaultActivity" runat="server" ClientInstanceName="cmbDefaultActivity" DataSourceID="ActivitiesXpoData"
                            ValueField="Oid" ValueType="System.Int32" Width="300px" OnValueChanged="OnCmbDefaultActivity_ValueChanged"
                            IncrementalFilteringMode="Contains" OnCallback="OnCmbDefaultActivity_Callback"
                            style="float:left; margin-right: 3px; margin-bottom: 3px;">
                    <Columns>
                        <dx:ListBoxColumn Caption="Nr." FieldName="ActivityNumber" Width="20%" />
                        <dx:ListBoxColumn Caption="Bezeichnung" FieldName="ActivityName" Width="80%" />
                    </Columns>
                </dx:ASPxComboBox>
                    </td>
                </tr>
            </table>
        </div>
        <div>
            <dx:aspxgridview ID="RecordGrid" runat="server" style="float:left;" 
                KeyFieldName="Id" AutoGenerateColumns="False" Width="100%" >
                <Columns>
                    <dx:GridViewCommandColumn VisibleIndex="0" ShowSelectCheckbox="True" ShowClearFilterButton="true">
                    </dx:GridViewCommandColumn>
                    <dx:GridViewDataTextColumn FieldName="Id" Caption="ID" Visible="false"/>
                    <dx:GridViewDataColumn Caption="Datum" FieldName="Date" Width="100px">
                        <DataItemTemplate>
                            <dx:ASPxDateEdit runat="server" ID="deDate" Width="100%" Value='<%# Eval("Date")%>' EditFormatString="dd.MM.yyyy" />
                        </DataItemTemplate>
                    </dx:GridViewDataColumn>
                    <dx:GridViewDataColumn Caption="Dauer" FieldName="EffortTime" Width="70px">
                        <DataItemTemplate>
                            <dx:ASPxTimeEdit runat="server" ID="seEffort" Width="100%" Value='<%# Eval("Effort")%>' DateTime='<%# GetDateTimeFromHours(Eval("Effort"))%>'
                                NullText="00:00"/>
                        </DataItemTemplate>
                    </dx:GridViewDataColumn>
                    <dx:GridViewDataColumn Caption="Tätigkeit" FieldName="Activity">
                        <DataItemTemplate>
                            <dx:ASPxComboBox ID ="CmbActivity" runat="server" ClientInstanceName="cmbActivity" DataSourceID="ActivitiesXpoData" 
                                IncrementalFilteringMode="Contains"  EnableCallbackMode="true"
                                ValueField="Oid" ValueType="System.Int32"  Width="100%" OnCallback="OnCmbActivity_Callback">
                                <ClientSideEvents DropDown="function(s, e){ OnCmbActivityDropDown(s);}" />
                                <Columns>
                                    <dx:ListBoxColumn Caption="Nr." FieldName="ActivityNumber" Width="20%" />
                                    <dx:ListBoxColumn Caption="Bezeichnung" FieldName="ActivityName" Width="80%" />
                                </Columns>
                            </dx:ASPxComboBox>
                        </DataItemTemplate>
                    </dx:GridViewDataColumn>
                </Columns>
                <Templates>
                    <PreviewRow>
                        Text: 
                        <dx:ASPxMemo Width=100% ID="tbxNote" runat="server" Value='<%# Eval("Text")%>'   />        
                    </PreviewRow>
                </Templates>
                <Settings ShowPreview="true" />

            </dx:aspxgridview>
        </div>

        <div>
            <dx:ASPxButton style="float:right; margin-top:3px" runat="server" OnClick="Import" Text="Import abschließen" />
        </div>
    </fieldset>

    <dx:XpoDataSource ID="ProjectsXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Projects">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="UserXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.User">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="ActivitiesXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Activities">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="PsPCodeXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.PspCodes">
    </dx:XpoDataSource>
    </asp:Content>