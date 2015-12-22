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

<%@ Page Title="UserRecordOverview" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" Inherits="com.commend.tools.PZE.UserRecordOverview" Codebehind="UserRecordOverview.aspx.cs" 
    ValidateRequest="false"%>

<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Xpo.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Xpo" TagPrefix="dx" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script src="Scripts/shortcut.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">

        shortcut.add("CTRL+Right", function () {
            recordGrid.NextPage();
        });

        shortcut.add("CTRL+Left", function () {
            recordGrid.PrevPage();
        });


        function OnGetRowValues(values) {
            cmbProject.SetValue(values[0]);
        }
        function UpdateEditValues() {
            recordGrid.GetRowValues(recordGrid.GetFocusedRowIndex(), 'Project!Key', OnGetRowValues);
        }

        function OnProjectChanged(CmbProject) {
            cmbPsPCode.PerformCallback(cmbProject.GetValue().toString());
        }

        function DropDownClicked() {
            cmbPsPCode.PerformCallback(cmbProject.GetValue().toString());
        }

        function CustomButtonClicked() {
            if (confirm("Wollen Sie diese Buchung wirklich löschen?")) {
                recordGrid.DeleteRow(recordGrid.GetFocusedRowIndex());
            }
        }

        function button1_Click(s, e) {
            if (recordGrid.IsCustomizationWindowVisible())
                recordGrid.HideCustomizationWindow();
            else
                recordGrid.ShowCustomizationWindow();
            UpdateButton();
        }
        function grid_CustomizationWindowCloseUp(s, e) {
            UpdateButton();
        }
        function UpdateButton() {
            if (recordGrid.IsCustomizationWindowVisible()) {
                button1.SetEnabled(false);
            } else {
                button1.SetEnabled(true);
            }
        }

        function TimeEditGotFocus() {
            timeEdit.SelectAll();
        }

        /*function expandButton_Clicked(s, e) {
            recordGrid.ExpandAll();
        }

        function collapseButton_Clicked(s, e) {
            recordGrid.CollapseAll();
        }*/
        
    </script>
    <fieldset>
        <legend>Buchungsauswertung</legend>
        <table width="100%">
            <tr>
                <td align="left">
                    <div align="left">
                        <table>
                            <tr>
                                <td>
                                    <dx:ASPxButton runat="server" ID="ASPxButton1" Text=" " Image-Url="Icons/ungroup.png"
                                        EnableDefaultAppearance="false" Cursor="pointer" Border-BorderColor="Transparent"
                                        BackColor="Transparent" OnClick="grid_UnGrouping" ToolTip="Gruppierung aufheben"
                                        PressedStyle-BackColor="Transparent">
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton runat="server" ID="ExportButton" Text="" AutoPostBack="false" Width="30px"
                                        Border-BorderColor="Transparent" BackColor="Transparent" EnableDefaultAppearance="false"
                                        Cursor="pointer" UseSubmitBehavior="false" Visible="true" AllowFocus="true" Image-Url="Icons/export.png"
                                        OnClick="OnExportButton_Click" ToolTip="Exportieren">
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxComboBox runat="server" ID="CmbExport" ClientInstanceName="cmbExport" ClientIDMode="Static"
                                        DropDownStyle="DropDownList" IncrementalFilteringMode="Contains" AutoPostBack="false"
                                        Width="100px" ToolTip="wählen Sie ein geeignetes Format">
                                        <Items>
                                            <dx:ListEditItem Text="Pdf" Value="Pdf"></dx:ListEditItem>
                                            <dx:ListEditItem Text="Xls" Value="Xls" Selected="true"></dx:ListEditItem>
                                            <dx:ListEditItem Text="Xlsx" Value="Xlsx"></dx:ListEditItem>
                                            <dx:ListEditItem Text="Rtf" Value="Rtf"></dx:ListEditItem>
                                            <dx:ListEditItem Text="Csv" Value="Csv"></dx:ListEditItem>
                                        </Items>
                                    </dx:ASPxComboBox>
                                </td>
                                <td>
                                    <dx:ASPxButton runat="server" ID="ExpandButton" AutoPostBack="false" Border-BorderColor="Transparent"
                                        BackColor="Transparent" EnableDefaultAppearance="false" UseSubmitBehavior="false"
                                        Visible="true" AllowFocus="true" ToolTip="Expand All" Image-Url="Icons/ExpandAll.png" OnClick="ExpandButton_Clicked">
                                    </dx:ASPxButton>
                                </td>
                                <td>
                                    <dx:ASPxButton runat="server" ID="CollapseButton" AutoPostBack="false" Border-BorderColor="Transparent"
                                        BackColor="Transparent" EnableDefaultAppearance="false" UseSubmitBehavior="false"
                                        Visible="true" AllowFocus="true" ToolTip="Collapse All" Image-Url="Icons/CollapseAll.png" OnClick="CollapseButton_Clicked">
                                    </dx:ASPxButton>
                                </td>
                                <td style=" width: 250px " align="left" >
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
                <%--<td width="50px">
                    <dx:ASPxLabel ID="UserLabel" runat="server" Text="Mitarbeiter" Visible="false">
                    </dx:ASPxLabel>
                </td>--%>
                <%--                <td style="white-space: nowrap" width="200px">
                    <dx:ASPxComboBox runat="server" ID="CmbUser" ClientInstanceName="cmbUser" ClientIDMode="Static"
                        DataSourceID="UserXpoData" DropDownStyle="DropDownList" TextFormatString="{0} {1} ({2})"
                        ValueField="Oid" ValueType="System.Int32" IncrementalFilteringMode="Contains"
                        EnableSynchronization="False" Width="200px" AutoPostBack="true" ToolTip="Mitarbeiter auswählen"
                        Visible="false">
                        <Columns>
                            <dx:ListBoxColumn FieldName="ForName" Caption="Vorname" />
                            <dx:ListBoxColumn FieldName="SurName" Caption="Nachname" />
                            <dx:ListBoxColumn FieldName="PersonalId" Caption="Personal Nr" />
                        </Columns>
                    </dx:ASPxComboBox>--%>
                <td >
                    <dx:ASPxLabel ID="FromToLabel" runat="server" Text="Von/Bis">
                    </dx:ASPxLabel>
                </td>
                <td >
                    <dx:ASPxDateEdit ID="FromDayEdit" runat="server" CalendarProperties-TodayStyle-Border-BorderColor="#0077C0"
                        CalendarProperties-DaySelectedStyle-BackColor="#0077C0" Width="123px" AllowNull="true"
                        ToolTip="Startdatum" TabIndex="1">
                    </dx:ASPxDateEdit>
                </td>
                <td>
                    <dx:ASPxDateEdit ID="ToDayEdit" runat="server" CalendarProperties-TodayStyle-Border-BorderColor="#0077C0"
                        CalendarProperties-DaySelectedStyle-BackColor="#0077C0" Width="123px" AllowNull="true"
                        ToolTip="Enddatum" TabIndex="2">
                    </dx:ASPxDateEdit>
                </td>
                <td>
                    <dx:ASPxButton runat="server" ID="ApplyButton" Text="" AutoPostBack="false" Width="30px"
                        Border-BorderColor="Transparent" BackColor="Transparent" EnableDefaultAppearance="false"
                        Cursor="pointer" UseSubmitBehavior="false" Visible="true" AllowFocus="true" Image-Url="Icons/Update_16x16.png"
                        OnClick="OnApplyButtonClicked" ToolTip="Zeitraum-Filter anwenden">
                    </dx:ASPxButton>
                </td>
            </tr>
        </table>
        <div>
            <dx:ASPxGridView ID="RecordGrid" ClientInstanceName="recordGrid" ClientIDMode="Static"
                runat="server" align="center" KeyFieldName="Oid" KeyboardSupport="true" Width="100%"
                DataSourceID="RecordsXpoData" AutoGenerateColumns="false" OnRowUpdating="RecordGridRowUpdating"
                OnCustomSummaryCalculate="CustomSummaryCalculate" EnableViewState="false" OnClientLayout="RecordGrid_ClientLayout">
                <SettingsCommandButton>
                    <EditButton Image-Url="Icons/Design-01.png"/>
                </SettingsCommandButton>
                <Columns>
                    <dx:GridViewCommandColumn VisibleIndex="0" Caption=" " Visible="false" ShowClearFilterButton="false"
                        ShowEditButton="false">
                        <CustomButtons>
                            <dx:GridViewCommandColumnCustomButton Text="Delete" ID="del" Visibility="Invisible" Image-Url="Icons/Delete-01_red_16x16.png">
                            </dx:GridViewCommandColumnCustomButton>
                        </CustomButtons>
                    </dx:GridViewCommandColumn>
                    <dx:GridViewDataTextColumn FieldName="User.ForName" Caption="Benutzer" Visible="false"
                        EditFormSettings-Visible="false" ShowInCustomizationForm="false">
                    </dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn FieldName="Oid" Caption="UserId" Visible="false" ShowInCustomizationForm="false">
                    </dx:GridViewDataTextColumn>
                    <dx:GridViewDataTextColumn VisibleIndex="0" FieldName="Date.DayOfWeek" Caption="Tag"
                        EditFormSettings-Visible="False" Visible="True">
                    </dx:GridViewDataTextColumn>
                    <dx:GridViewDataDateColumn VisibleIndex="1" FieldName="Date" Caption="Datum" EditFormSettings-VisibleIndex="1" SortOrder="Descending">
                        <EditItemTemplate>
                            <dx:ASPxDateEdit ID="DayEdit" runat="server" Date='<%# Eval("Date") %>' CalendarProperties-TodayStyle-Border-BorderColor="#0077C0"
                                CalendarProperties-DaySelectedStyle-BackColor="#0077C0" Width="153px" AllowNull="false">
                            </dx:ASPxDateEdit>
                        </EditItemTemplate>
                    </dx:GridViewDataDateColumn>
                    <dx:GridViewDataComboBoxColumn VisibleIndex="3" FieldName="Project!Key" Caption="Proj. Nr"
                        EditFormSettings-Visible="false">
                        <PropertiesComboBox DataSourceID="ProjectsXpoData" TextField="ProjectNumber" ValueField="Oid"
                            IncrementalFilteringMode="StartsWith" />
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataComboBoxColumn VisibleIndex="4" EditFormSettings-VisibleIndex="2"
                        FieldName="Project!Key" Caption="Projekt">
                        <PropertiesComboBox DataSourceID="ProjectsXpoData" TextField="ProjectName" ValueField="Oid"
                            IncrementalFilteringMode="StartsWith" Width="100px" />
                        <DataItemTemplate>
                            <div style="width: 80px; overflow: hidden; white-space: nowrap;" title='<%# GetProjectNameByOid(Eval("Project!Key")) %>'>
                                <%# GetProjectNameByOid(Eval("Project!Key"))%>
                            </div>
                        </DataItemTemplate>
                        <EditItemTemplate>
                            <dx:ASPxComboBox runat="server" ID="CmbProject" ClientInstanceName="cmbProject" ClientIDMode="Static"
                                Width="100%" DropDownStyle="DropDownList" IncrementalFilteringMode="Contains"
                                Value='<%# Eval("Project!Key") %>' DataSourceID="ProjectsXpoData" TextFormatString="{0} ({1})"
                                ValueField="Oid" ValueType="System.Int32" EnableSynchronization="False">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="ProjectName" />
                                    <dx:ListBoxColumn FieldName="ProjectNumber" />
                                </Columns>
                                <ClientSideEvents SelectedIndexChanged="function(s, e) { OnProjectChanged(s); }" />
                            </dx:ASPxComboBox>
                        </EditItemTemplate>
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataComboBoxColumn VisibleIndex="5" FieldName="PspCode!Key" Caption="PSP Code"
                        EditFormSettings-Visible="False">
                        <PropertiesComboBox DataSourceID="PspCodesXpoData" TextField="PspCodeNumber" ValueField="Oid"
                            IncrementalFilteringMode="StartsWith" />
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataComboBoxColumn VisibleIndex="6" EditFormSettings-VisibleIndex="4"
                        FieldName="PspCode!Key" Caption="PSP Bezeichnung" EditFormSettings-Caption="PSP Code">
                        <PropertiesComboBox DataSourceID="PspCodesXpoData" TextField="PspCodeName" ValueField="Oid"
                            IncrementalFilteringMode="StartsWith" />
                        <DataItemTemplate>
                            <div style="width: 80px; overflow: hidden; white-space: nowrap;" title='<%# GetPsPCodeNameByOid(Eval("PspCode!Key")) %>'>
                                <%# GetPsPCodeNameByOid(Eval("PspCode!Key")) %>
                            </div>
                        </DataItemTemplate>
                        <EditItemTemplate>
                            <dx:ASPxComboBox ID="CmbPspCode" ClientInstanceName="cmbPsPCode" ClientIDMode="Static"
                                runat="server" Width="100%" DropDownStyle="DropDownList" Value='<%# Eval("PspCode!Key") %>'
                                DataSourceID="PspCodesXpoData" ValueField="Oid" ValueType="System.Int32" TextFormatString="{0} ({1})"
                                EnableCallbackMode="true" IncrementalFilteringMode="Contains" CallbackPageSize="30"
                                OnCallback="CmbPsPCode_Callback">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="PspCodeName" />
                                    <dx:ListBoxColumn FieldName="PspCodeNumber" />
                                </Columns>
                                <ClientSideEvents DropDown="DropDownClicked" />
                            </dx:ASPxComboBox>
                        </EditItemTemplate>
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataComboBoxColumn VisibleIndex="7" FieldName="Activity!Key" Caption="Tät. Nr"
                        EditFormSettings-Visible="False">
                        <PropertiesComboBox DataSourceID="ActivitiesXpoData" TextField="ActivityNumber" ValueField="Oid"
                            IncrementalFilteringMode="StartsWith" />
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataComboBoxColumn VisibleIndex="8" EditFormSettings-VisibleIndex="6"
                        FieldName="Activity!Key" Caption="Tätigkeit">
                        <PropertiesComboBox DataSourceID="ActivitiesXpoData" TextField="ActivityName" ValueField="Oid"
                            IncrementalFilteringMode="StartsWith" />
                        <DataItemTemplate>
                            <div style="width: 60px; overflow: hidden; white-space: nowrap;" title='<%# GetActivityNameByOid(Eval("Activity!Key")) %>'>
                                <%# GetActivityNameByOid(Eval("Activity!Key"))%>
                            </div>
                        </DataItemTemplate>
                        <EditItemTemplate>
                            <dx:ASPxComboBox ID="CmbActivity" runat="server" Width="100%" DropDownStyle="DropDown"
                                Value='<%# Eval("Activity!Key") %>' DataSourceID="ActivitiesXpoData" ValueField="Oid"
                                ValueType="System.Int32" TextFormatString="{0} ({1})" EnableCallbackMode="true"
                                IncrementalFilteringMode="Contains" CallbackPageSize="30">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="ActivityName" />
                                    <dx:ListBoxColumn FieldName="ActivityNumber" />
                                </Columns>
                            </dx:ASPxComboBox>
                        </EditItemTemplate>
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataMemoColumn VisibleIndex="9" FieldName="Memo" Caption="Notiz" EditFormSettings-VisibleIndex="5">
                        <DataItemTemplate>
                            <div style="width: 60px; overflow: hidden; white-space: nowrap;" title="<%# Eval("Memo") %>">
                                <%# Eval("Memo") %>
                            </div>
                        </DataItemTemplate>
                    </dx:GridViewDataMemoColumn>
                    <dx:GridViewDataTextColumn VisibleIndex="2" FieldName="Duration" Caption="Dauer"
                        EditFormSettings-VisibleIndex="3">
                        <DataItemTemplate>
                            <%# GetHoursAndMinutesFromMinutes(Eval("Duration"))%>
                        </DataItemTemplate>
                        <EditItemTemplate>
                            <table>
                                <tr>
                                    <td>
                                        <dx:ASPxTimeEdit ID="TimeEdit" ClientInstanceName="timeEdit" ClientIDMode="Static"
                                            runat="server" Width="153px" DateTime='<%# GeDateTimeFromMinutes(Eval("Duration"))%>'>
                                            <ValidationSettings ErrorDisplayMode="None" />
                                            <ClientSideEvents GotFocus="TimeEditGotFocus" />
                                        </dx:ASPxTimeEdit>
                                    </td>
                                </tr>
                            </table>
                        </EditItemTemplate>
                    </dx:GridViewDataTextColumn>
                </Columns>
                <SettingsEditing PopupEditFormWidth="600" PopupEditFormModal="true" NewItemRowPosition="Bottom" />
                <Templates>
                    <EditForm>
                        <div style="padding: 4px 4px 3px 4px">
                            <dx:ASPxPageControl runat="server" ID="pageControl" Width="100%">
                                <TabPages>
                                    <dx:TabPage Text="Edit" Visible="true">
                                        <ContentCollection>
                                            <dx:ContentControl ID="ContentControl1" runat="server">
                                                <dx:ASPxGridViewTemplateReplacement ID="Editors" ReplacementType="EditFormEditors"
                                                    runat="server"></dx:ASPxGridViewTemplateReplacement>
                                            </dx:ContentControl>
                                        </ContentCollection>
                                    </dx:TabPage>
                                </TabPages>
                            </dx:ASPxPageControl>
                        </div>
                        <div style="text-align: right; padding: 2px 2px 2px 2px">
                            <dx:ASPxGridViewTemplateReplacement ID="UpdateButton" ReplacementType="EditFormUpdateButton"
                                runat="server"></dx:ASPxGridViewTemplateReplacement>
                            <dx:ASPxGridViewTemplateReplacement ID="CancelButton" ReplacementType="EditFormCancelButton"
                                runat="server"></dx:ASPxGridViewTemplateReplacement>
                        </div>
                    </EditForm>
                </Templates>
                <Styles FocusedRow-BackColor="#0077C0" FocusedGroupRow-BackColor="#0077C0">
                    <AlternatingRow Enabled="true" />
                </Styles>
                <SettingsBehavior AllowSort="true" />
                <Settings ShowFilterRow="True" EnableFilterControlPopupMenuScrolling="true" ShowGroupPanel="True"
                    ShowFooter="True" ShowGroupFooter="VisibleIfExpanded"/>
                <SettingsText GroupPanel="Um zu gruppieren, ziehen Sie bitte die entsprechende Spalte hier her"
                    CommandUpdate="Save" CommandClearFilter="clear Filter" CustomizationWindowCaption="Versteckte Spalten" />
                <SettingsPager PageSize="25" Visible="true"/>
                <SettingsCustomizationWindow Enabled="True" />
                <ClientSideEvents CustomButtonClick="CustomButtonClicked" CustomizationWindowCloseUp="grid_CustomizationWindowCloseUp" />
                <GroupSummary>
                    <dx:ASPxSummaryItem FieldName="Duration" ShowInGroupFooterColumn="Duration" SummaryType="Custom" />
                </GroupSummary>
                <TotalSummary>
                     <dx:ASPxSummaryItem FieldName="Duration" ShowInGroupFooterColumn="Duration" SummaryType="Custom" />
                </TotalSummary>
            </dx:ASPxGridView>
        </div>
    </fieldset>
    <%--    <IFRAME id="frame1" src="http://ws2000time/webterm/"  runat="server" width="500" height="300" frameborder="0"/>--%>
   <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="RecordGrid" OnRenderBrick="OnRenderBrick"
        PaperKind="A4" Landscape="true" MaxColumnWidth="200">
        <Styles>
            <Cell Font-Size="Small">
            </Cell>
        </Styles>
    </dx:ASPxGridViewExporter>
    <dx:XpoDataSource ID="RecordsXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Records">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="ProjectsXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Projects">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="ActivitiesXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Activities">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="PspCodesXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.PspCodes">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="UserXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.User">
    </dx:XpoDataSource>
</asp:Content>
