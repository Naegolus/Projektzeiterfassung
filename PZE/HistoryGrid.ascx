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

<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Xpo.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Xpo" TagPrefix="dx" %>

<script src="Scripts/shortcut.js" type="text/javascript"></script>
<script type="text/javascript">

    shortcut.add("CTRL+E", function () {
        var selectionIndex = historyGrid.GetFocusedRowIndex();
        if (selectionIndex > -1) {
            historyGrid.GetRowValues(selectionIndex, 'EmployeeLocked', OnRowEdit);
        } else {
            alert("Kein Eintrag ausgewählt!");
        }
    });

    shortcut.add("delte", function () {
        var selectionIndex = historyGrid.GetFocusedRowIndex();
        if (selectionIndex > -1) {
            historyGrid.GetRowValues(selectionIndex, 'EmployeeLocked', OnRowDelete);
        } else {
            alert("Kein Eintrag ausgewählt!");
        }
    });

    shortcut.add("CTRL+S", function () {
        historyGrid.UpdateEdit();
    });

    shortcut.add("CTRL+C", function () {
        historyGrid.CancelEdit();
    });

    shortcut.add("esc", function () {
        historyGrid.CancelEdit();
    });

    function OnRowEdit(Value) {
        if (Value != null) {
            alert("Dieser Eintrag wurde bereits freigegeben und kann daher nicht mehr verändert werden.");
        }
        else {
            historyGrid.StartEditRow(historyGrid.GetFocusedRowIndex());
        }
    }

    function OnRowDelete(Value) {
        if (Value != null) {
            alert("Dieser Eintrag wurde bereits freigegeben und kann daher nicht mehr verändert werden.");
        }
        else {
            if (confirm("Wollen Sie diese Buchung wirklich löschen?")) {
                historyGrid.DeleteRow(historyGrid.GetFocusedRowIndex());
            }
        }
    }

    function OnHryProjectChanged(HryCmbProject) {
        hryCmbPsPCode.PerformCallback(hryCmbProject.GetValue().toString());
        hryCmbActivity.PerformCallback(HryCmbProject.GetValue().toString());
    }

    function OnHryEditProjectInit(s) {
        var projectId = s.GetValue();
        if (projectId !== null) {
            hryCmbPsPCode.PerformCallback(projectId.toString());
            hryCmbActivity.PerformCallback(projectId.toString());
        }
    }

    function HryTimeEditGotFocus() {
        hryTimeEdit.SelectAll();
    }

    function HryCustomButtonClicked() {
        if (confirm("Wollen Sie diese Buchung wirklich löschen?")) {
            historyGrid.SetFocusedRowIndex(historyGrid.GetFocusedRowIndex());
            historyGrid.DeleteRow(historyGrid.GetFocusedRowIndex());
        }
    }

    function UpdateEditValuesByHistory(s, e) {
        if (
            historyGrid.GetFocusedRowIndex() > -1) {
            var focusedRowIndex = historyGrid.GetFocusedRowIndex();
            historyGrid.GetRowValues(focusedRowIndex, 'Project!Key;PspCode!Key;Activity!Key;Memo', OnGetRowValues);
        }
    }

    function HryRowClick(s, e) {
        var newIndex = e.visibleIndex,
            oldIndex = historyGrid.GetFocusedRowIndex();

        if ((newIndex === oldIndex) && (historyGrid.GetFocusedRowIndex() > -1)) {
            historyGrid.GetRowValues(historyGrid.GetFocusedRowIndex(), 'Project!Key;PspCode!Key;Activity!Key;Memo', OnGetRowValues);
        }
    }

    var update = false;
    function historyGridBeginCallback(s, e) {
        if (e.command == 'UPDATEEDIT'
            || e.command == 'DELETEROW') {
            update = true;
        }
        else {
            update = false;
        }
    }

    function updateCalendar(s, e) {
        if (update === true) {
            reloadDaySummaryData();            
        }
    }

    function onGridInit(s, e) {
        s.SetHeight(400);
    }
    
</script>

<%@ Control Language="C#" AutoEventWireup="false" Inherits="com.commend.tools.PZE.History" CodeBehind="HistoryGrid.ascx.cs" %>
<dx:ASPxGridView ID="HistoryGrid" runat="server" ClientInstanceName="historyGrid"
    ClientIDMode="Static" align="center" KeyFieldName="Oid" KeyboardSupport="True"
    Width="100%" AutoGenerateColumns="False" AutoPostBack="false" OnRowUpdating="HistoryGridRowUpdating"
    OnRowDeleting="HistoryGridRowDeleting" EnableViewState="False" OnDataBinding="HistoryGrid_DataBinding"
    OnCommandButtonInitialize="HistoryGrid_CommandButtonInit" OnCustomButtonInitialize="HistoryGrid_CustomButtonInit"
    OnCustomSummaryCalculate="CustomSummaryCalculate">
    <SettingsCommandButton>
        <EditButton ButtonType="Image">
            <Image Url="~/Icons/edit.png" ToolTip="edit" />
        </EditButton>
        <UpdateButton ButtonType="Image">
            <Image ToolTip="Save changes and close Edit Form" Url="~/Icons/update.png" />
        </UpdateButton>
        <CancelButton ButtonType="Image">
            <Image ToolTip="Close Edit Form without saving changes" Url="~/Icons/cancel.png" />
        </CancelButton>
    </SettingsCommandButton>
    <ClientSideEvents BeginCallback="historyGridBeginCallback" EndCallback="updateCalendar" Init="onGridInit"/>
    <Columns>
        <dx:GridViewCommandColumn VisibleIndex="0" Width="60" Visible="false" ShowEditButton="true" ButtonType="Image">
            <CustomButtons>
                <dx:GridViewCommandColumnCustomButton ID="del" Image-Url="~/Icons/delete.png" Image-AlternateText="delete" Image-ToolTip="delete">
                </dx:GridViewCommandColumnCustomButton>
            </CustomButtons>
        </dx:GridViewCommandColumn>
        <dx:GridViewDataTextColumn FieldName="User.ForName" Caption="Benutzer" Visible="false">
        </dx:GridViewDataTextColumn>
        <dx:GridViewDataTextColumn FieldName="Date.DayOfWeek" Caption="Tag" EditFormSettings-Visible="False"
            Visible="False">
            <EditFormSettings Visible="False"></EditFormSettings>
        </dx:GridViewDataTextColumn>
        <dx:GridViewDataColumn VisibleIndex="1" FieldName="Date" Caption="Datum" Settings-FilterMode="Value" Width="75px">
            <EditItemTemplate>
                <dx:ASPxDateEdit ID="DayEdit" runat="server" Date='<%# Eval("Date") %>' CalendarProperties-TodayStyle-Border-BorderColor="#0077C0"
                    CalendarProperties-DaySelectedStyle-BackColor="#0077C0" Width="153px" AllowNull="false">
                </dx:ASPxDateEdit>
            </EditItemTemplate>
        </dx:GridViewDataColumn>
        <dx:GridViewDataTextColumn VisibleIndex="2" FieldName="Duration" Caption="Dauer"
            ToolTip="Dauer [h:min]" EditFormSettings-VisibleIndex="3" Width="60">
            <EditFormSettings VisibleIndex="3"></EditFormSettings>
            <DataItemTemplate>
                <%# GetHoursAndMinutesFromMinutes(Eval("Duration"))%>
                        </div>
            </DataItemTemplate>
            <EditItemTemplate>
                <table>
                    <tr>
                        <td>
                            <dx:ASPxTimeEdit ID="HryTimeEdit" ClientInstanceName="hryTimeEdit" ClientIDMode="Static"
                                runat="server" Width="153px" DateTime='<%# GeDateTimeFromMinutes(Eval("Duration"))%>'>
                                <ValidationSettings ErrorDisplayMode="None" />
                                <ClientSideEvents GotFocus="HryTimeEditGotFocus" />
                            </dx:ASPxTimeEdit>
                        </td>
                    </tr>
                </table>
            </EditItemTemplate>
        </dx:GridViewDataTextColumn>
        <dx:GridViewDataComboBoxColumn VisibleIndex="3" FieldName="Project!Key" Caption="Projekt Nr"
            EditFormSettings-Visible="false" Visible="true" Width="65px">
            <PropertiesComboBox DataSourceID="ProjectsXpoData" TextField="ProjectNumber" ValueField="Oid"
                IncrementalFilteringMode="StartsWith" />
            <EditFormSettings Visible="False"></EditFormSettings>
        </dx:GridViewDataComboBoxColumn>
        <dx:GridViewDataComboBoxColumn VisibleIndex="4" EditFormSettings-VisibleIndex="2"
            FieldName="Project!Key" Caption="Projekt" CellStyle-Wrap="False" MinWidth="120">
            <PropertiesComboBox DataSourceID="ProjectsXpoData" TextField="ProjectName" ValueField="Oid"
                IncrementalFilteringMode="StartsWith" />
            <EditFormSettings VisibleIndex="2"></EditFormSettings>
            <DataItemTemplate>
                <div style="width: 100%; overflow: hidden; white-space: nowrap;" title='<%# GetProjectNameByOid(Eval("Project!Key")) %>'>
                    <%# GetProjectNameByOid(Eval("Project!Key"))%>
                </div>
            </DataItemTemplate>
            <EditItemTemplate>
                <dx:ASPxComboBox runat="server" ID="HryCmbProject" ClientInstanceName="hryCmbProject"
                    ClientIDMode="Static" Width="100%" DropDownStyle="DropDownList" IncrementalFilteringMode="Contains"
                    Value='<%# Eval("Project!Key") %>' DataSourceID="ProjectsXpoData" TextFormatString="{0} ({1})"
                    ValueField="Oid" ValueType="System.Int32" EnableSynchronization="False">
                    <Columns>
                        <dx:ListBoxColumn FieldName="ProjectName" />
                        <dx:ListBoxColumn FieldName="ProjectNumber" />
                    </Columns>
                    <ClientSideEvents SelectedIndexChanged="function(s, e) { OnHryProjectChanged(s); }"
                        Init="OnHryEditProjectInit" />
                </dx:ASPxComboBox>
            </EditItemTemplate>
            <CellStyle Wrap="False"></CellStyle>
        </dx:GridViewDataComboBoxColumn>
        <dx:GridViewDataComboBoxColumn VisibleIndex="5" FieldName="PspCode!Key" Caption="PSP Code"
            EditFormSettings-Visible="False" Visible="false">
            <PropertiesComboBox DataSourceID="HryCmbPsPCodeXpoData" TextField="PspCodeNumber" ValueField="Oid"
                        IncrementalFilteringMode="StartsWith"/>
            <EditFormSettings Visible="False"></EditFormSettings>
                </dx:GridViewDataComboBoxColumn>
        <dx:GridViewDataComboBoxColumn VisibleIndex="6" EditFormSettings-VisibleIndex="4"
            FieldName="PspCode!Key" Caption="PSP Bezeichnung" EditFormSettings-Caption="PSP Code"
            MinWidth="180">
                    <PropertiesComboBox DataSourceID="HryCmbPsPCodeXpoData" TextField="PspCodeName" ValueField="Oid"
                        IncrementalFilteringMode="StartsWith"/>
            <EditFormSettings VisibleIndex="4" Caption="PSP Code"></EditFormSettings>
            <DataItemTemplate>
                <div style="width: 100%;" title='<%# GetPsPCodeNameByOid(Eval("Project!Key"), Eval("PspCode!Key")) %>'>
                    <%# GetPsPCodeNameByOid(Eval("Project!Key"), Eval("PspCode!Key")) %>
                </div>
            </DataItemTemplate>
            <EditItemTemplate>
                <dx:ASPxComboBox ID="HryCmbPspCode" ClientInstanceName="hryCmbPsPCode" ClientIDMode="Static"
                    runat="server" Width="100%" DropDownStyle="DropDownList" Value='<%# Eval("PspCode!Key") %>'
                    DataSourceID="HryCmbPsPCodeXpoData" ValueField="Oid" ValueType="System.Int32"
                    TextFormatString="{0} ({1})" EnableCallbackMode="true" IncrementalFilteringMode="Contains"
                    CallbackPageSize="30" OnCallback="HryCmbPspCode_Callback">
                    <Columns>
                        <dx:ListBoxColumn FieldName="PspCodeName" />
                        <dx:ListBoxColumn FieldName="PspCodeNumber" />
                    </Columns>
                </dx:ASPxComboBox>
            </EditItemTemplate>
            <CellStyle Wrap="False"></CellStyle>
                </dx:GridViewDataComboBoxColumn>
        <dx:GridViewDataComboBoxColumn VisibleIndex="7" FieldName="Activity!Key" Caption="Tätigkeits Nr" Visible="false"
            EditFormSettings-Visible="False">
                    <PropertiesComboBox DataSourceID="ActivitiesXpoData" TextField="ActivityNumber" ValueField="Oid"
                        IncrementalFilteringMode="StartsWith"/>
            <EditFormSettings Visible="False"></EditFormSettings>
        </dx:GridViewDataComboBoxColumn>
        <dx:GridViewDataComboBoxColumn VisibleIndex="8" EditFormSettings-VisibleIndex="6"
            FieldName="Activity!Key" Caption="Tätigkeit" CellStyle-Wrap="False" MinWidth="110">
            <PropertiesComboBox DataSourceID="HryCmbActivity" TextField="ActivityName" ValueField="Oid"
                IncrementalFilteringMode="StartsWith" />
            <EditFormSettings VisibleIndex="6"></EditFormSettings>
            <DataItemTemplate>
                <div style="width: 100%" title='<%# GetActivityNameByOid(Eval("Project!Key"), Eval("Activity!Key")) %>'>
                    <%# GetActivityNameByOid(Eval("Project!Key"), Eval("Activity!Key"))%>
                </div>
            </DataItemTemplate>
            <EditItemTemplate>
                <dx:ASPxComboBox ID="HryCmbActivity" ClientInstanceName="hryCmbActivity" runat="server" Width="100%" DropDownStyle="DropDown"
                    Value='<%# Eval("Activity!Key") %>' DataSourceID="ActivitiesXpoData" ValueField="Oid"
                    ValueType="System.Int32" TextFormatString="{0} ({1})" EnableCallbackMode="true"
                    IncrementalFilteringMode="Contains" CallbackPageSize="30" OnCallback="HryCmbActivity_Callback">
                    <Columns>
                        <dx:ListBoxColumn FieldName="ActivityName" />
                        <dx:ListBoxColumn FieldName="ActivityNumber" />
                    </Columns>
                </dx:ASPxComboBox>
            </EditItemTemplate>
            <CellStyle Wrap="False"></CellStyle>
                </dx:GridViewDataComboBoxColumn>
        <dx:GridViewDataMemoColumn VisibleIndex="9" FieldName="Memo" Caption="Notiz" EditFormSettings-VisibleIndex="5"
            CellStyle-Wrap="False" MinWidth="200">
            <EditFormSettings VisibleIndex="5"></EditFormSettings>
            <DataItemTemplate>
                <div style="min-width: 100%" title="<%# Eval("Memo") %>">
                    <%# Eval("Memo") %>
                </div>
            </DataItemTemplate>
            <CellStyle Wrap="False"></CellStyle>
        </dx:GridViewDataMemoColumn>

        <dx:GridViewDataColumn VisibleIndex="10" FieldName="EmployeeLocked" Caption="MA Freig." Width="80px" />
        <dx:GridViewDataColumn VisibleIndex="11" FieldName="LeaderLocked" Caption="LT Freig." Width="80px" />
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
    <ClientSideEvents CustomButtonClick="HryCustomButtonClicked" FocusedRowChanged="UpdateEditValuesByHistory"
        RowClick="HryRowClick" />
    <SettingsText CommandUpdate="Save" />
    <SettingsBehavior AllowFocusedRow="true" />
    <Settings VerticalScrollBarMode="Visible" VerticalScrollBarStyle="Virtual" VerticalScrollableHeight="300" />
    <Styles>
        <AlternatingRow Enabled="true" />
    </Styles>
    <SettingsPager>
        <PageSizeItemSettings Visible="false" />
    </SettingsPager>
</dx:ASPxGridView>

<dx:XpoDataSource ID="ProjectsXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Projects">
</dx:XpoDataSource>
<dx:XpoDataSource ID="ActivitiesXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Activities">
</dx:XpoDataSource>
<dx:XpoDataSource ID="HryCmbPsPCodeXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.PspCodes">
</dx:XpoDataSource>