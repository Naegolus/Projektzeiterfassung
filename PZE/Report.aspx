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

<%@ Page Title="Report" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="False" Inherits="com.commend.tools.PZE.Report"
    CodeBehind="Report.aspx.cs" ValidateRequest="false" %>

<%@ Register Assembly="DevExpress.XtraReports.v15.1.Web, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.XtraReports.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Xpo.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Xpo" TagPrefix="dx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <%--<link href="~/Styles/Report.css" rel="stylesheet" type="text/css" />--%>
    <style type="text/css">
        .showColumnsRow {
            display: table-row;
            height: 25px;
        }

        .showColumnsCell {
            display: table-cell;
            width: 150px;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
    </asp:ScriptManager>
    <script src="Scripts/shortcut.js" type="text/javascript"></script>
    <script type="text/javascript">
        shortcut.add("CTRL+Right", function () {
            reportGrid.NextPage();
        });

        shortcut.add("CTRL+Left", function () {
            reportGrid.PrevPage();
        });

        function FilterApplied(s, e) {
            reportGrid.ApplyFilter(e.filterExpression);
        }

        function ValueChanged(s, e) {
            var gridview = s.GetGridView();
            gridview.GetSelectedFieldValues('Oid', OnGetSelectedFieldValues);
        }

        function OnGetSelectedFieldValues(values) {
            var filterexpression = "";
            for (var i = 0; i < values.length; i++) {
                if (i == 0) {
                    filterexpression = filterexpression + "[User!Key] = " + values[0];
                }
                else {
                    filterexpression = filterexpression + " OR [User!Key] = " + values[i];
                }
            }
        }

        function onBtnEditFilterClick() {
            reportGrid.ShowFilterControl();
        }

        function onSettingsLegendClick(sender) {
            var settingsContent = document.getElementById('settingsContent');

            if (settingsContent.style.display === 'none') {
                settingsContent.style.display = 'block';
                sender.innerHTML = 'Einstellungen [-]';
            }
            else {
                settingsContent.style.display = 'none';
                sender.innerHTML = 'Einstellungen [+]';
            }
        }

        function showSetNamePoput(sender) {
            if (sender === btnRename) {
                filterNamePopup.SetHeaderText('Rename');
                hiddenField.Set('Mode', 'Rename')
            }
            else if (sender === btnSaveNew) {
                filterNamePopup.SetHeaderText('New Filter');
                hiddenField.Set('Mode', 'New')
            }
            filterNamePopup.Show();
        }

        function showSettings() {
            if (hiddenField.Get('ShowSettings') === true) {
                settingsContent.style.display = 'block';
                settingsLegend.innerHTML = 'Einstellungen [-]';
            }
        }

    </script>
    <div>
        <fieldset>
            <legend id="settingsLegend" onclick="onSettingsLegendClick(this)">Einstellungen [+]</legend>
            <div id="settingsContent" style="display: none">
                <fieldset>
                    <legend>Filter</legend>
                    <div style="display: inline-flex; width: 100%">
                        <div style="display: flex; margin-right: 20px; width: 33%">
                            <dx:ASPxComboBox runat="server" ID="CmbEditFilter"
                                DropDownStyle="DropDownList" TextFormatString="{0}" OnSelectedIndexChanged="OnCmbEditFilterSelectedIndexChanged"
                                ValueField="Oid" ValueType="System.Int32" IncrementalFilteringMode="Contains"
                                AutoPostBack="true" Width="75%" TabIndex="3" Enabled="true" OnInit="OnCmbFilterInit">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="FilterName" Caption="Filtername" />
                                </Columns>
                            </dx:ASPxComboBox>
                        </div>
                        <div style="display: flex; margin-bottom: 10px; margin-right: 20px; width: 66%">                           
                            <div style="display: flex; width: 20%">
                                <dx:ASPxButton runat="server" ID="BtnSaveFilter" OnClick="OnBtnSaveFilterClick" Width="100%" Text="Save"
                                    ToolTip="Überschreibe Filter">
                                    <Image Url="~/Icons/save.png" />
                                </dx:ASPxButton>
                            </div>
                            <div style="display: flex; width: 20%">
                                <dx:ASPxButton runat="server" AutoPostBack="false" ClientInstanceName="btnRename" Width="100%"
                                    Text="Rename" ToolTip="Umbenennen">
                                    <Image Url="~/Icons/text.png" />
                                    <ClientSideEvents Click="showSetNamePoput" />
                                </dx:ASPxButton>
                            </div>
                            <div style="display: flex; width: 20%">
                                <dx:ASPxButton runat="server" ClientInstanceName="btnSaveNew" Width="100%" Text="Save As New"
                                    ToolTip="Speichere als neuen Filter" AutoPostBack="false">
                                    <Image Url="~/Icons/add.png" />
                                    <ClientSideEvents Click="showSetNamePoput" />
                                </dx:ASPxButton>
                            </div>
                        </div>
                    </div>
                    <div style="display: inline-flex; width: 100%">
                        <dx:ASPxFilterControl ID="FilterControl" runat="server">
                            <Columns>
                                <dx:FilterControlColumn PropertyName="User.SurName" DisplayName="Nachname">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="User.Division.DivisionName" DisplayName="Bereich">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="User.PersonalId" DisplayName="Pers. Nr">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="Date" DisplayName="Datum">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="Duration" DisplayName="Dauer">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="Project.ProjectNumber" DisplayName="Proj. Nr">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="Project.ProjectName" DisplayName="Projekt">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="PspCode.PspCodeNumber" DisplayName="PSP Code">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="PspCode.PspCodeName" DisplayName="PSP Bezeichnung">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="Activity.ActivityNumber" DisplayName="Tät. Nr">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="Activity.ActivityName" DisplayName="Tätigkeit">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="Memo" DisplayName="Notiz">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="User.ForName" DisplayName="Vorname">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="User.UserName" DisplayName="Benutzer">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="EmployeeLocked" DisplayName="Mit. Freig.">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="LeaderLocked" DisplayName="LT. Freig.">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="Project.ResearchProject.ResearchProjectNumber" DisplayName="Res.Proj-Nr.">
                                </dx:FilterControlColumn>
                                <dx:FilterControlColumn PropertyName="Project.ResearchProject.ResearchProjectName" DisplayName="Res.Proj-Name.">
                                </dx:FilterControlColumn>
                            </Columns>
                        </dx:ASPxFilterControl>
                    </div>
                </fieldset>
                <fieldset>
                    <legend>Grid formartieren</legend>
                    <div class="showColumnsRow">
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckDivision" runat="server" Text="Bereich" AutoPostBack="false"
                                Checked="false" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckPersNr" runat="server" Text="Pers. Nr" AutoPostBack="false"
                                Checked="false" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckUserSurName" runat="server" Text="Nachname" AutoPostBack="false"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckUserForName" runat="server" Text="Vorname" AutoPostBack="false"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckDate" runat="server" Text="Datum" AutoPostBack="false"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckDuration" runat="server" Text="Dauer" AutoPostBack="false"
                                Checked="true" EnableViewState="false" />
                        </div>
                    </div>
                    <div class="showColumnsRow">
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckActivityNr" runat="server" Text="Tät. Nr" AutoPostBack="false"
                                Checked="false" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckActivityName" runat="server" Text="Tätigkeit" AutoPostBack="false"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckMemo" runat="server" Text="Notiz" AutoPostBack="false"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckPSPCode" runat="server" Text="PSP Code" AutoPostBack="false"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckPSPName" runat="server" Text="PSP Bezeichung"
                                Checked="True" EnableViewState="False" CheckState="Checked" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckExternalStaff" runat="server" Text="Ext. Mitarbeiter"
                                Checked="false" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxButton runat="server" ID="ExpandButton" AutoPostBack="false" Text="Zeilen ausklappen"
                                Border-BorderColor="Transparent" BackColor="Transparent" EnableDefaultAppearance="false"
                                UseSubmitBehavior="false" Visible="true" AllowFocus="true" ToolTip="Expand All" Image-Url="Icons/ExpandAll.png"
                                OnClick="ExpandButton_Clicked">
                            </dx:ASPxButton>
                        </div>
                    </div>
                    <div class="showColumnsRow">
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckProjNr" runat="server" Text="Proj. Nr" AutoPostBack="false"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckProjName" runat="server" Text="Projekt Name" AutoPostBack="false"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckResearchProjNumber" runat="server" Text="Forsch. Proj-Nr." AutoPostBack="false"
                                Checked="false" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckResearchProjName" runat="server" Text="Forsch. Proj-Name" AutoPostBack="false"
                                Checked="true" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckEmployeeRelease" runat="server" Text="Mit. Freig."
                                Checked="false" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckLeaderRelease" runat="server" Text="LT. Freig."
                                Checked="false" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxButton runat="server" ID="CollapseButton" AutoPostBack="false" Text="Zeilen einklappen"
                                Border-BorderColor="Transparent" BackColor="Transparent" EnableDefaultAppearance="false"
                                UseSubmitBehavior="false" Visible="true" AllowFocus="true" ToolTip="Collapse All" Image-Url="Icons/CollapseAll.png"
                                OnClick="CollapseButton_Clicked">
                            </dx:ASPxButton>
                        </div>
                    </div>
                    <div class="showColumnsRow">
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckProjectType" runat="server" Text="Projektart"
                                Checked="false" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckProjResearchPercentage" runat="server" Text="Forsch. Anteil"
                                Checked="false" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="ChecProjkActivatable" runat="server" Text="Proj. Aktivierbar"
                                Checked="false" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckProjCostEstimate" runat="server" Text="Proj. Kosten"
                                Checked="false" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckProjStartDate" runat="server" Text="Proj. Start"
                                Checked="false" EnableViewState="false" />
                        </div>
                        <div class="showColumnsCell">
                            <dx:ASPxCheckBox ID="CheckProjectEndDate" runat="server" Text="Proj. Ende"
                                Checked="false" EnableViewState="false" />
                        </div>
                    </div>
                </fieldset>
            </div>
        </fieldset>
    </div>
    <fieldset>
        <legend>Report</legend>
        <div style="display: inline-flex">
            <div style="display: inline-flex; width: 100%">
                <div style="display: flex; margin-bottom: 10px; width: 30%; margin-right: 30px">
                    <dx:ASPxLabel ID="FilterLabel" runat="server" Text="Filter" Width="20%">
                    </dx:ASPxLabel>
                    <dx:ASPxComboBox runat="server" ID="CmbFilter" ClientInstanceName="cmbFilter" ClientIDMode="Static"
                        DropDownStyle="DropDownList" TextFormatString="{0}"
                        ValueField="Oid" ValueType="System.Int32" IncrementalFilteringMode="Contains"
                        AutoPostBack="false" Width="80%" TabIndex="3" Enabled="true" OnInit="OnCmbFilterInit">
                        <Columns>
                            <dx:ListBoxColumn FieldName="FilterName" Caption="Filtername" />
                        </Columns>
                    </dx:ASPxComboBox>
                </div>
                <div style="display: flex; margin-bottom: 10px; width: 60%">
                    <dx:ASPxLabel ID="FromToLabel" runat="server" Text="Von/Bis" Width="10%">
                    </dx:ASPxLabel>
                    <dx:ASPxDateEdit ID="FromDayEdit" runat="server" ClientInstanceName="fromDayEdit"
                        CalendarProperties-DaySelectedStyle-BackColor="#0077C0" Width="25%" AllowNull="true"
                        ToolTip="Startdatum" TabIndex="1" OnInit="OnFromDayEditInit" CalendarProperties-TodayStyle-Border-BorderColor="#0077C0">
                    </dx:ASPxDateEdit>
                    <dx:ASPxDateEdit ID="ToDayEdit" runat="server" ClientInstanceName="toDayEdit"
                        CalendarProperties-DaySelectedStyle-BackColor="#0077C0" Width="25%" AllowNull="true"
                        ToolTip="Enddatum" TabIndex="2" OnInit="OnToDayEditInit" CalendarProperties-TodayStyle-Border-BorderColor="#0077C0">
                    </dx:ASPxDateEdit>
                    <dx:ASPxButton runat="server" ID="PreviewButton" AutoPostBack="False" Paddings-Padding="0" Width="20%"
                        UseSubmitBehavior="False" OnClick="OnApplyButtonClicked" EnableDefaultAppearance="false"
                        TabIndex="8" Text="Anwenden">
                        <FocusRectPaddings Padding="0" />
                        <Image Url="~/Icons/filter.png" />
                    </dx:ASPxButton>
                </div>
                <div style="display: flex; margin-bottom: 10px">
                    <dx:ASPxComboBox runat="server" ID="CmbExport" ClientInstanceName="cmbExport" ClientIDMode="Static"
                        DropDownStyle="DropDownList" IncrementalFilteringMode="Contains" AutoPostBack="false"
                        Width="100px">
                        <Items>
                            <dx:ListEditItem Text="Pdf" Value="Pdf"></dx:ListEditItem>
                            <dx:ListEditItem Text="Xls" Value="Xls"></dx:ListEditItem>
                            <dx:ListEditItem Text="Xlsx" Value="Xlsx" Selected="true"></dx:ListEditItem>
                            <dx:ListEditItem Text="Rtf" Value="Rtf"></dx:ListEditItem>
                            <dx:ListEditItem Text="Csv" Value="Csv"></dx:ListEditItem>
                        </Items>
                    </dx:ASPxComboBox>
                    <dx:ASPxButton runat="server" Text="Filter" AutoPostBack="false" ToolTip="Export Filter"
                        Image-Url="Icons/export.png" OnClick="OnExportFilterClick">
                        <Paddings Padding="0" PaddingLeft="5px" PaddingRight="5px" />
                        <FocusRectPaddings Padding="0" />
                    </dx:ASPxButton> 
                    <dx:ASPxButton runat="server" ID="ExportButton" Text="View" AutoPostBack="false" ToolTip="Export View"
                        Image-Url="Icons/export.png" OnClick="OnExportButton_Click">
                        <Paddings Padding="0" PaddingLeft="5px" PaddingRight="5px" />
                        <FocusRectPaddings Padding="0" />
                    </dx:ASPxButton>                    
                </div>
            </div>

        </div>
        <dx:ASPxGridView ID="ReportGrid" ClientInstanceName="reportGrid" ClientIDMode="Static"
            runat="server" align="center" KeyFieldName="Oid" EnableViewState="false"
            KeyboardSupport="true" Width="100%" OnCustomSummaryCalculate="CustomSummaryCalculate"
            OnBeforeColumnSortingGrouping="OnBeforeColumSortingGrouping">
            <Columns>
                <dx:GridViewDataColumn VisibleIndex="1" FieldName="User.SurName" Caption="Nachname">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="3" FieldName="User.Division.DivisionName" Caption="Bereich" Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="4" FieldName="User.PersonalId" Caption="Pers. Nr" Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="6" FieldName="Date" Caption="Datum">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="7" FieldName="Duration" Caption="Dauer">
                    <DataItemTemplate>
                        <div style="width: 40px; overflow: hidden; white-space: nowrap;" title="<%# GetHoursAndMinutesFromMinutes(Eval("Duration"))%>">
                            <%# GetHoursAndMinutesFromMinutes(Eval("Duration"))%>
                        </div>
                    </DataItemTemplate>
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="8" FieldName="Project.ProjectNumber" Caption="Proj. Nr">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="9" FieldName="Project.ProjectName" Caption="Projekt">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="10" FieldName="PspCode.PspCodeNumber" Caption="PSP Code">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="11" FieldName="PspCode.PspCodeName" Caption="PSP Bezeichnung">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="12" FieldName="Activity.ActivityNumber" Caption="Tät. Nr" Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="13" FieldName="Activity.ActivityName" Caption="Tätigkeit">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="14" FieldName="Memo" Caption="Notiz">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="2" FieldName="User.ForName" Caption="Vorname">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="15" FieldName="User.UserName" Caption="Benutzer" Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="16" FieldName="EmployeeLocked" Caption="Mit. Freig." Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="17" FieldName="LeaderLocked" Caption="LT. Freig." Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="18" FieldName="Project.ResearchProject.ResearchProjectNumber" Caption="Res.Proj-Nr." Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="19" FieldName="Project.ResearchProject.ResearchProjectName" Caption="Res.Proj-Name.">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="20" FieldName="User.ExternalStaff" Caption="Ext. Mitarbeiter" Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="21" FieldName="Project.ProjectType.Name" Caption="Projektart" Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="22" FieldName="Project.ResearchPercentage" Caption="Forschungsanteil" Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="23" FieldName="Project.Activatable" Caption="Proj. Aktivierbar" Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="24" FieldName="Project.CostEstimate" Caption="Projektkosten" Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="25" FieldName="Project.StartDate" Caption="Start-Datum" Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="26" FieldName="Project.EndDate" Caption="End-Datum" Visible="false">
                </dx:GridViewDataColumn>
            </Columns>
            <Styles>
                <AlternatingRow Enabled="true" />
            </Styles>
            <Settings ShowGroupPanel="True" ShowFooter="True" ShowGroupFooter="VisibleIfExpanded" GroupFormat="{1}{2} "
                ShowFilterRow="true" HorizontalScrollBarMode="Visible"
                VerticalScrollBarMode="Visible" VerticalScrollBarStyle="Virtual" VerticalScrollableHeight="600" />
            <SettingsText GroupPanel=" " GroupContinuedOnNextPage="(weitere Einträge befinden sich auf der nächsten Seite)" />
            <SettingsPager PageSize="40" Visible="true" />
            <SettingsBehavior ColumnResizeMode="NextColumn" />
            <TotalSummary>
                <dx:ASPxSummaryItem FieldName="Duration" ShowInGroupFooterColumn="Duration" SummaryType="Custom" />
            </TotalSummary>
            <GroupSummary>
                <dx:ASPxSummaryItem FieldName="Duration" ShowInGroupFooterColumn="Duration" SummaryType="Custom" />
            </GroupSummary>
        </dx:ASPxGridView>
    </fieldset>
    <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="ReportGrid"
        PaperKind="A4" Landscape="true" MaxColumnWidth="200" OnRenderBrick="OnRenderBrick">
        <Styles>
            <Cell Font-Size="Small">
            </Cell>
        </Styles>
    </dx:ASPxGridViewExporter>
    <dx:ASPxHiddenField runat="server" ClientInstanceName="hiddenField" ID="HiddenField">
        <ClientSideEvents Init="showSettings" />
        </dx:ASPxHiddenField>
    <dx:ASPxPopupControl ID="FilterNamePopup" HeaderText="test" runat="server" ClientInstanceName="filterNamePopup"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter" Modal="true" CloseAction="CloseButton">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">                
                <div style="display: inline-flex; margin-bottom: 10px">
                    <dx:ASPxLabel runat="server" Text="Filtername: "></dx:ASPxLabel>
                    <dx:ASPxTextBox ID="TENewFilterName" runat="server"></dx:ASPxTextBox>
                </div>
                <dx:ASPxButton runat="server" Text="Save" OnClick="OnSaveFilterNameClick">
                </dx:ASPxButton>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
</asp:Content>
