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

<%@ Page Title="Buchen" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="false" Inherits="com.commend.tools.PZE.UserRecords"
    CodeBehind="UserRecords.aspx.cs" ValidateRequest="false" %>

<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Xpo.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Xpo" TagPrefix="dx" %>

<%@ Register TagPrefix="my" TagName="HistoryGrid" Src="~/HistoryGrid.ascx" %>
<%@ Register TagPrefix="my" TagName="Legend" Src="~/ColorLegendeEmployee.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
    </asp:ScriptManager>
    <script src="Scripts/shortcut.js" type="text/javascript"></script>
    <script src="Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">

        function getTotalMinutesFromDateTime(dateTime) {
            return dateTime.getHours() * 60 + dateTime.getMinutes();
        }

        function setSubmitButtonEnableState() {
            var project = cmbProject.GetValue(),
                pspCode = cmbPsPCode.GetValue(),
                activity = cmbActivity.GetValue(),
                txt = memo.GetValue(),
                diffTimeDate = actualDayDifferenceTime.GetDate(),
                newTime = getTotalMinutesFromDateTime(timeEdit.GetDate()),
                timeValid = false;

            if (externalEmployee.GetChecked() === true) {
                timeValid = true;
            }
            else if (diffTimeDate !== null && newTime !== 0
                && newTime <= getTotalMinutesFromDateTime(diffTimeDate)) {
                timeValid = true;
            }

            if (project && pspCode && activity && txt && timeValid) {
                submitButton.SetEnabled(true);
                submitButton.mainElement.style.background = '#0081C7';
                submitButton.mainElement.style.color = '#FFFFFF';
            }
            else {
                submitButton.mainElement.style.backgroundColor = '';
                submitButton.mainElement.style.color = '';
                submitButton.SetEnabled(false);
            }
        }

        function OnGetRowValues(values) {
            var projectId = values[0],
                pspCodeId = values[1] !== null ? values[1] : -1,
                activityId = values[2] !== null ? values[2] : -1,
                memoVal = values[3] !== null ? values[3] : 'null',
                selectedProjectId = cmbProject.GetValue(),
                selectedPsPCodeId = cmbPsPCode.GetValue(),
                selectedActivityId = cmbActivity.GetValue(),
                writtenMemo = memo.GetValue();

            cbpBuchung.PerformCallback([projectId, pspCodeId, activityId, memoVal]);
            setSubmitButtonEnableState();
        }

        function UpdateEditValuesByFavorites() {
            if (favoritesGrid.GetFocusedRowIndex() > -1) {
                favoritesGrid.GetRowValues(favoritesGrid.GetFocusedRowIndex(), 'Project!Key;PspCode!Key;Activity!Key;Memo', OnGetRowValues);
            }
        }

        function FavRowClick(s, e) {
            var newIndex = e.visibleIndex,
                oldIndex = favoritesGrid.GetFocusedRowIndex();

            if ((newIndex === oldIndex) && (favoritesGrid.GetFocusedRowIndex() > -1)) {
                favoritesGrid.GetRowValues(favoritesGrid.GetFocusedRowIndex(), 'Project!Key;PspCode!Key;Activity!Key;Memo', OnGetRowValues);
            }
        }

        function onCalBuchungClick(s) {
            var today = new Date(),
                date = s.GetSelectedDate();
            if (date < today) {
                teDate.SetDate(date);
                actualDaySummaryCallback.PerformCallback(s);
                setSubmitButtonEnableState();
            }
            else {
                alert('Sie können nicht in die Zukunft buchen');
            }
        }

        function onCalendarSelectionChanged(Calendar) {
            historyGridCallback.PerformCallback(Calendar);
            summaryFieldCallback.PerformCallback(Calendar);
        }

        function onProjectChanged(sender) {
            loadPspCodeAndActivitiesByProject(sender.GetValue());
            setSubmitButtonEnableState();
        }

        function loadPspCodeAndActivitiesByProject(projectId) {
            if (projectId != null) {
                cbpBuchung.PerformCallback(projectId);
                setSubmitButtonEnableState();
            }
        }

        function onPsPCodeChanged() {
            setSubmitButtonEnableState();
        }

        function onActivityChanged() {
            setSubmitButtonEnableState();
        }

        function OnMemoTextChanged(memo) {
            setSubmitButtonEnableState();
        }

        function OnFavProjectChanged(sender) {
            favCmbPsPCode.PerformCallback(sender.GetValue().toString());
            favCmbActivity.PerformCallback(sender.GetValue().toString());
        }

        function FavCustomButtonClicked() {
            if (confirm("Wollen Sie diesen Favoriten wirklich löschen?")) {
                favoritesGrid.DeleteRow(favoritesGrid.GetFocusedRowIndex());
            }
        }

        function onFavCmbEditProjectInit(s, e) {
            var projectId = s.GetValue();
            if (projectId !== null) {
                favCmbPsPCode.PerformCallback(projectId.toString());
                favCmbActivity.PerformCallback(projectId.toString());
            }
        }

        function timeEditGotFocus() {
            timeEdit.SelectAll();
        }

        function onHourButtonClicked(s, e) {
            var hours = s.GetText();
            if (hours != null) {
                timeEdit.SetValue(new Date(0, 0, 0, parseInt(hours), 0, 0));
            }
            setSubmitButtonEnableState();
        }

        function onQuarterButtonClicked(s, e) {
            var date = timeEdit.GetDate() || new Date(0, 0, 0, 0, 0);

            var minutes = date.getMinutes();
            if (minutes < 45) {
                date.setMinutes(minutes + 15);
            }
            else {
                date.setMinutes(0);
            }

            timeEdit.SetDate(date);
            setSubmitButtonEnableState();
        }

        function onLeftTimeButtonClicked(s, e) {
            timeEdit.SetDate(actualDayDifferenceTime.GetDate());
            setSubmitButtonEnableState();
        }

        function onBuchungLegendClick(sender) {
            var content = document.getElementById('buchungsContent');
            if (content.style.display === 'none') {
                content.style.display = 'block';
                sender.innerHTML = 'Buchung [-]';
            }
            else {
                content.style.display = 'none';
                sender.innerHTML = 'Buchung [+]';
            }
        }

        function onFavoritesLegendClick(sender) {
            var favoritesContent = document.getElementById('favoritesContent');

            if (favoritesContent.style.display === 'none') {
                favoritesContent.style.display = 'block';
                sender.innerHTML = 'Favoriten [-]';
            }
            else {
                favoritesContent.style.display = 'none';
                sender.innerHTML = 'Favoriten [+]';
            }
        }

        function onOverviewAndFreeLegendClick(sender) {
            var overViewAndFreeContent = document.getElementById('overViewAndFreeContent');

            if (overViewAndFreeContent.style.display === 'none') {
                overViewAndFreeContent.style.display = 'block';
                sender.innerHTML = 'Buchungsübersicht und Freigabe [-]';
            }
            else {
                overViewAndFreeContent.style.display = 'none';
                sender.innerHTML = 'Buchungsübersicht und Freigabe [+]';
            }
        }

        function onTeDateInit(s) {
            if (s.GetDate() === null) {
                var today = new Date();
                s.SetDate(today);
            }
            actualDaySummaryCallback.PerformCallback();
            historyGridCallback.PerformCallback();  //TODO: Remove only temp. to load HistoryGrid after BUCHEN click
            tempTest(); //TODO: Remove only
        }

        function tempTest() {
            var selectedProjectId = cmbProject.GetValue(),
                selectedPsPCodeId = cmbPsPCode.GetValue(),
                selectedActivityId = cmbActivity.GetValue(),
                writtenMemo = memo.GetValue();

            if (selectedProjectId != null) {
                cbpBuchung.PerformCallback([selectedProjectId, selectedPsPCodeId, selectedActivityId, writtenMemo]);
            }
        }

        function reloadDaySummaryData() {
            actualDaySummaryCallback.PerformCallback();
            summaryFieldCallback.PerformCallback();
            recordCalendarCallback.PerformCallback();
        }

        function teDateLostFocus() {
            var date = new Date();
            date.setHours(0);
            date.setMinutes(0);
            timeEdit.SetDate(date);
            actualDaySummaryCallback.PerformCallback();
            setSubmitButtonEnableState();
        }

        function teHoursLostFocus() {
            actualDaySummaryCallback.PerformCallback();
            setSubmitButtonEnableState();
        }

    </script>
    <fieldset>
        <legend onclick="onBuchungLegendClick(this)">Buchung [-]</legend>
        <div id="buchungsContent" style="width: 100%">
            <div style="display: inline-flex; vertical-align: top; align-content: center; width: 25%; height: 100%">
                <dx:ASPxCalendar ID="CalBuchung" runat="server" ShowClearButton="false" ShowTodayButton="true" ShowWeekNumbers="true"
                    EnableMultiSelect="false" ToolTip="Bitte wählen Sie das gewünschte Datum ihrer Buchung" AutoPostBack="false"
                    TodayStyle-Border-BorderColor="#D61F01" DaySelectedStyle-BackColor="#0077C0">
                    <DaySelectedStyle BackColor="#0077C0" />
                    <ClientSideEvents SelectionChanged="onCalBuchungClick" />
                    <TodayStyle>
                        <Border BorderColor="#D61F01"></Border>
                    </TodayStyle>
                </dx:ASPxCalendar>
            </div>
            <div style="display: inline-flex; vertical-align: bottom; width: 53%">
                <dx:ASPxCallbackPanel runat="server" ClientInstanceName="cbpBuchung" OnCallback="OnCbpBuchungCallback"
                    RenderMode="Table" Width="100%">
                    <PanelCollection>
                        <dx:PanelContent>
                            <div style="display: table; width: 100%">
                                <div runat="server" id="newRecordUserRow" style="display: table-row">
                                    <div class="newRecordLabel">
                                        <dx:ASPxLabel ID="UserLabel" runat="server" Text="Mitarbeiter" Visible="false" CssClass="newRecordLabelFont" Font-Size="Larger">
                                        </dx:ASPxLabel>
                                    </div>
                                    <div class="newRecordInput">
                                        <dx:ASPxComboBox runat="server" ID="CmbUser" CssClass="newRecordInputFont"
                                            DropDownStyle="DropDownList" TextFormatString="{0} {1} ({2})"
                                            ValueField="Oid" ValueType="System.Int32" IncrementalFilteringMode="Contains"
                                            EnableSynchronization="False" Visible="false" AutoPostBack="false"
                                            OnValueChanged="CmbUser_ValueChanged">
                                            <Columns>
                                                <dx:ListBoxColumn FieldName="ForName" Caption="Vorname" Width="150px" />
                                                <dx:ListBoxColumn FieldName="SurName" Caption="Nachname" Width="150px" />
                                                <dx:ListBoxColumn FieldName="PersonalId" Caption="Personal Nr" Width="70px" />
                                            </Columns>
                                        </dx:ASPxComboBox>
                                    </div>
                                </div>
                                <div id="newRecordTeamMembersRow" runat="server" style="display: table-row">
                                    <div class="newRecordLabel">
                                        <dx:ASPxLabel ID="LblTeamMembers" runat="server" Text="Team-Mitglied " Visible="false" CssClass="newRecordLabelFont">
                                        </dx:ASPxLabel>
                                    </div>
                                    <div class="newRecordInput">
                                        <dx:ASPxGridLookup ID="TeamMembersLookup" runat="server" SelectionMode="Multiple"
                                            ClientIDMode="Static" KeyFieldName="Oid" IncrementalFilteringMode="Contains" Visible="false"
                                            TextFormatString="{0} {1} ({2})" MultiTextSeparator=", " CssClass="newRecordInputFont">
                                            <Columns>
                                                <dx:GridViewCommandColumn ShowSelectCheckbox="True" Width="40px" />
                                                <dx:GridViewDataColumn FieldName="ForName" Caption="Vorname" Width="150px" />
                                                <dx:GridViewDataColumn FieldName="SurName" Caption="Nachname" Width="150px" />
                                                <dx:GridViewDataColumn FieldName="PersonalId" Caption="Personal Nr" Width="70px" />
                                            </Columns>
                                            <GridViewProperties>
                                                <Settings ShowFilterRow="True" ShowStatusBar="Visible" UseFixedTableLayout="true" />
                                            </GridViewProperties>
                                        </dx:ASPxGridLookup>
                                    </div>
                                </div>
                                <div id="newRecordProjectRow" runat="server" style="display: table-row">
                                    <div class="newRecordLabel">
                                        <dx:ASPxLabel ID="ASPxLabel2" runat="server" Text="Projekt" CssClass="newRecordLabelFont">
                                        </dx:ASPxLabel>
                                    </div>
                                    <div class="newRecordInput">
                                        <dx:ASPxComboBox runat="server" ID="CmbProject" ClientInstanceName="cmbProject" ClientIDMode="Static"
                                            DropDownStyle="DropDownList" IncrementalFilteringMode="Contains" CssClass="newRecordInputFont"
                                            TextFormatString="{1} ({0})" ValueField="Oid" ValueType="System.Int32"
                                            EnableSynchronization="False" TabIndex="1">
                                            <ClientSideEvents SelectedIndexChanged="function(s, e) { onProjectChanged(s); }" />
                                            <Columns>
                                                <dx:ListBoxColumn FieldName="ProjectNumber" Caption="Projekt Nr" Width="70px" />
                                                <dx:ListBoxColumn FieldName="ProjectName" Caption="Projekt Name" Width="300px" />
                                            </Columns>
                                        </dx:ASPxComboBox>
                                    </div>
                                </div>
                                <div id="newReocordPspCodeRow" runat="server" style="display: table-row">
                                    <div class="newRecordLabel">
                                        <dx:ASPxLabel ID="ASPxLabel3" runat="server" Text="PSP Code" CssClass="newRecordLabelFont">
                                        </dx:ASPxLabel>
                                    </div>
                                    <div class="newRecordInput">
                                        <dx:ASPxComboBox runat="server" ID="CmbPsPCode" ClientInstanceName="cmbPsPCode" ClientIDMode="Static"
                                            DropDownStyle="DropDownList" TextFormatString="{1} ({0})" CssClass="newRecordInputFont"
                                            ValueField="Oid" ValueType="System.Int32" IncrementalFilteringMode="Contains"
                                            EnableSynchronization="False" TabIndex="2">
                                            <ClientSideEvents SelectedIndexChanged="function(s, e){ onPsPCodeChanged(s);}" />
                                            <Columns>
                                                <dx:ListBoxColumn FieldName="PspCodeNumber" Caption="PSP Code" Width="70px" />
                                                <dx:ListBoxColumn FieldName="PspCodeName" Caption="PSP Bezeichnung" Width="300px" />
                                            </Columns>
                                        </dx:ASPxComboBox>
                                    </div>
                                </div>
                                <div id="newRecordActivityRow" runat="server" style="display: table-row">
                                    <div class="newRecordLabel">
                                        <dx:ASPxLabel ID="ASPxLabel4" runat="server" Text="Tätigkeit" CssClass="newRecordLabelFont">
                                        </dx:ASPxLabel>
                                    </div>
                                    <div class="newRecordInput">
                                        <dx:ASPxComboBox runat="server" ID="CmbActivity" ClientInstanceName="cmbActivity"
                                            ClientIDMode="Static" DropDownStyle="DropDown" CssClass="newRecordInputFont"
                                            TextFormatString="{1} ({0})" ValueField="Oid" ValueType="System.Int32" IncrementalFilteringMode="Contains"
                                            EnableSynchronization="False" TabIndex="3">
                                            <ClientSideEvents SelectedIndexChanged="function(s, e){ onActivityChanged(s);}" />
                                            <Columns>
                                                <dx:ListBoxColumn FieldName="ActivityNumber" Caption="Tätigkeits Nr" Width="70px" />
                                                <dx:ListBoxColumn FieldName="ActivityName" Caption="Tätigkeit" Width="300px" />
                                            </Columns>
                                        </dx:ASPxComboBox>
                                    </div>
                                </div>
                                <div id="newReocordMemoRow" runat="server" style="display: table-row">
                                    <div class="newRecordLabel">
                                        <dx:ASPxLabel ID="ASPxLabel10" runat="server" Text="Notiz" CssClass="newRecordLabelFont">
                                        </dx:ASPxLabel>
                                    </div>
                                    <div class="newRecordInput" style="white-space: normal">
                                        <dx:ASPxMemo ID="ASPxMemo1" runat="server" Height="71px" ClientInstanceName="memo"
                                            TabIndex="4" CssClass="newRecordInputFont" >
                                            <ClientSideEvents TextChanged="function(s, e){ OnMemoTextChanged(s);}" />
                                        </dx:ASPxMemo>
                                    </div>
                                </div>
                                <div style="display: none">
                                    <dx:ASPxCheckBox runat="server" ID="ExternalEmployee" ClientInstanceName="externalEmployee" Checked="false" />
                                </div>
                            </div>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </div>
            <div style="display: inline-flex; vertical-align: bottom; width: 19%; margin-bottom: 5px">
                <div style="width: 100%">
                    <div style="display: block">
                        <div class="bookingSummaryField">
                            <dx:ASPxLabel runat="server" Text="Datum:" Font-Size="Larger" />
                        </div>
                        <div style="display: inline-flex; margin-bottom: 0px">
                            <dx:ASPxTimeEdit runat="server" ID="TENewRecordDate" CssClass="newRecordInputFont" ClientInstanceName="teDate"
                                SpinButtons-ShowIncrementButtons="false" Width="100px" ForeColor="#696969" Border-BorderStyle="None"
                                EditFormat="Date" Enabled="true" TabIndex="5">
                                <ClientSideEvents Init="onTeDateInit" LostFocus="teDateLostFocus" />
                            </dx:ASPxTimeEdit>
                        </div>
                    </div>
                    <dx:ASPxCallbackPanel runat="server" ClientInstanceName="actualDaySummaryCallback" OnCallback="OnCbpBuchungActualDaySummaryCallback">
                        <PanelCollection>
                            <dx:PanelContent>
                                <div class="bookingSummaryField"></div>

                                <div style="display: block">
                                    <div class="bookingSummaryField">
                                        <dx:ASPxLabel runat="server" Text="Anwesend:" Visible="true" Font-Size="Larger" />
                                    </div>
                                    <div style="display: inline-flex; margin-bottom: 0px">
                                        <dx:ASPxTimeEdit ID="ActualDayAttendanceTime" runat="server" EditFormat="Time" Enabled="false" CssClass="newRecordInputFont"
                                            SpinButtons-ShowIncrementButtons="false" Width="100px" ForeColor="#696969" Border-BorderStyle="None" />
                                    </div>
                                </div>
                                <div style="display: block">
                                    <div class="bookingSummaryField">
                                        <dx:ASPxLabel runat="server" Text="Gebucht:" Visible="true" Font-Size="Larger" />
                                    </div>
                                    <div style="display: inline-flex; margin-bottom: 0px">
                                        <dx:ASPxTimeEdit ID="ActualDayBookedTime" runat="server" EditFormat="Time" Enabled="false" CssClass="newRecordInputFont"
                                            SpinButtons-ShowIncrementButtons="false" Width="100px" ForeColor="#696969" Border-BorderStyle="None" />
                                    </div>
                                </div>
                                <div style="display: block">
                                    <div class="bookingSummaryField">
                                        <dx:ASPxLabel runat="server" Text="Differenz:" Visible="true" Font-Size="Larger" />
                                    </div>
                                    <div style="display: inline-flex; margin-bottom: 0px">
                                        <dx:ASPxTimeEdit ID="ActualDayDifferenceTime" ClientInstanceName="actualDayDifferenceTime" runat="server" EditFormat="Time" Enabled="false"
                                            CssClass="newRecordInputFont"
                                            SpinButtons-ShowIncrementButtons="false" Width="100px" ForeColor="#696969" Border-BorderStyle="None" />
                                    </div>
                                </div>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxCallbackPanel>
                    <div class="bookingSummaryField"></div>
                    <div style="display: table; width: 100%">
                        <div style="display: table-cell; width: 12.5%">
                            <dx:ASPxButton runat="server" Text="1" AutoPostBack="false" UseSubmitBehavior="false"
                                Theme="Moderno" Paddings-Padding="0" Font-Size="Large">
                                <ClientSideEvents Click="onHourButtonClicked" />
                            </dx:ASPxButton>
                        </div>
                        <div style="display: table-cell; width: 12.5%">

                            <dx:ASPxButton runat="server" Text="2" AutoPostBack="false" UseSubmitBehavior="false"
                                Theme="Moderno" Paddings-Padding="0" Font-Size="Large">
                                <ClientSideEvents Click="onHourButtonClicked" />
                            </dx:ASPxButton>
                        </div>
                        <div style="display: table-cell; width: 12.5%">
                            <dx:ASPxButton runat="server" Text="3" AutoPostBack="false" UseSubmitBehavior="false"
                                Theme="Moderno" Paddings-Padding="0" Font-Size="Large">
                                <ClientSideEvents Click="onHourButtonClicked" />
                            </dx:ASPxButton>
                        </div>
                        <div style="display: table-cell; width: 12.5%">
                            <dx:ASPxButton runat="server" Text="4" AutoPostBack="false" UseSubmitBehavior="false"
                                Theme="Moderno" Paddings-Padding="0" Font-Size="Large">
                                <ClientSideEvents Click="onHourButtonClicked" />
                            </dx:ASPxButton>
                        </div>
                        <div style="display: table-cell; width: 12.5%">
                            <dx:ASPxButton runat="server" Text="5" AutoPostBack="false" UseSubmitBehavior="false"
                                Theme="Moderno" Paddings-Padding="0" Font-Size="Large">
                                <ClientSideEvents Click="onHourButtonClicked" />
                            </dx:ASPxButton>
                        </div>
                        <div style="display: table-cell; width: 12.5%">
                            <dx:ASPxButton runat="server" Text="6" AutoPostBack="false" UseSubmitBehavior="false"
                                Theme="Moderno" Paddings-Padding="0" Font-Size="Large">
                                <ClientSideEvents Click="onHourButtonClicked" />
                            </dx:ASPxButton>
                        </div>
                        <div style="display: table-cell; width: 12.5%">
                            <dx:ASPxButton runat="server" Text="7" AutoPostBack="false" UseSubmitBehavior="false"
                                Theme="Moderno" Paddings-Padding="0" Font-Size="Large">
                                <ClientSideEvents Click="onHourButtonClicked" />
                            </dx:ASPxButton>
                        </div>
                        <div style="display: table-cell; width: 12.5%">
                            <dx:ASPxButton runat="server" Text="8" AutoPostBack="false" UseSubmitBehavior="false"
                                Theme="Moderno" Paddings-Padding="0" Font-Size="Large">
                                <ClientSideEvents Click="onHourButtonClicked" />
                            </dx:ASPxButton>
                        </div>
                    </div>
                    <div style="display: table; width: 100%; margin-top: 5px">
                        <div style="display: table-cell; width: 49%">
                            <dx:ASPxButton runat="server" Text="+15 min" AutoPostBack="false" UseSubmitBehavior="false"
                                Theme="Moderno" Font-Size="Large" Paddings-Padding="0" Width="100%">
                                <ClientSideEvents Click="onQuarterButtonClicked" />
                            </dx:ASPxButton>
                        </div>
                        <div style="display: table-cell; width: 2%"></div>
                        <div style="display: table-cell; width: 49%">
                            <dx:ASPxButton runat="server" Text="Differenz" AutoPostBack="false" UseSubmitBehavior="false"
                                Theme="Moderno" Font-Size="Large" Paddings-Padding="0" Width="100%" TabIndex="6">
                                <ClientSideEvents Click="onLeftTimeButtonClicked" />
                            </dx:ASPxButton>
                        </div>
                    </div>
                    <div style="display: table; width: 100%; margin-top: 5px">
                        <div style="display: table-cell; width: 49%; vertical-align: top;">
                            <dx:ASPxTimeEdit ID="TimeEdit" ClientInstanceName="timeEdit" ClientIDMode="Static" runat="server" DateTime="00:00"
                                Height="50px" SpinButtons-ShowIncrementButtons="false" Theme="Moderno" Width="100%" ClearButton-Visibility="True"
                                Font-Size="Large" Font-Bold="true" ForeColor="#696969" BackColor="#90EE90" TabIndex="7">
                                <ValidationSettings ErrorDisplayMode="None" />
                                <ClientSideEvents GotFocus="timeEditGotFocus" LostFocus="teHoursLostFocus" />
                            </dx:ASPxTimeEdit>
                        </div>
                        <div style="display: table-cell; width: 2%"></div>
                        <div style="display: table-cell; width: 49%; vertical-align: top;">
                            <dx:ASPxButton ID="SubmitButton" ClientInstanceName="submitButton" runat="server" Width="100%"
                                Text="BUCHEN" UseSubmitBehavior="false" OnClick="storeRecordToDB" AllowFocus="true" Font-Size="Large"
                                AutoPostBack="false" EnableClientSideAPI="true" Height="50px" TabIndex="8"
                                Theme="Moderno" Font-Bold="true">
                                <Paddings Padding="0" />
                                <ClientSideEvents Init="setSubmitButtonEnableState" />
                            </dx:ASPxButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </fieldset>
    <fieldset>
        <legend id="favoritesLegend" onclick="onFavoritesLegendClick(this)">Favoriten [+]
        </legend>
        <div id="favoritesContent" style="display: none">
            <dx:ASPxGridView ID="FavoritesGrid" runat="server" ClientInstanceName="favoritesGrid"
                ClientIDMode="Static" KeyFieldName="Oid" KeyboardSupport="true"
                Width="100%" AutoGenerateColumns="False" AutoPostBack="false" OnRowUpdating="FavoritesGridRowUpdating"
                OnRowInserting="FavoritesGridRowInserting"
                OnRowDeleting="FavoritesGridRowDeleteing" EnableViewState="false" OnDataBinding="FavoritesGrid_DataBinding">
                <SettingsCommandButton>
                    <EditButton ButtonType="Image">
                        <Image Url="~/Icons/edit.png" ToolTip="edit" />
                    </EditButton>
                    <NewButton ButtonType="Image">
                        <Image Url="~/Icons/add.png" ToolTip="new" />
                    </NewButton>
                    <UpdateButton ButtonType="Image">
                        <Image ToolTip="Save changes and close Edit Form" Url="Icons/update.png" />
                    </UpdateButton>
                    <CancelButton ButtonType="Image">
                        <Image ToolTip="Close Edit Form without saving changes" Url="Icons/cancel.png" />
                    </CancelButton>
                </SettingsCommandButton>
                <Columns>
                    <dx:GridViewCommandColumn VisibleIndex="0" ShowEditButton="true" ShowNewButtonInHeader="True" ButtonType="Image">
                        <CustomButtons>
                            <dx:GridViewCommandColumnCustomButton ID="FavoritesGridCommandColumnCustomButton">
                                <Image Url="~/Icons/delete.png" ToolTip="delete" />
                            </dx:GridViewCommandColumnCustomButton>
                        </CustomButtons>
                    </dx:GridViewCommandColumn>
                    <dx:GridViewDataTextColumn FieldName="User.ForName" Caption="Benutzer" Visible="false" EditFormSettings-Visible="false">
                    </dx:GridViewDataTextColumn>
                    <dx:GridViewDataComboBoxColumn VisibleIndex="1" FieldName="Project!Key" Caption="Projekt Nr"
                        EditFormSettings-Visible="false" Visible="true">
                        <PropertiesComboBox DataSourceID="ProjectsXpoData" TextField="ProjectNumber" ValueField="Oid"
                            IncrementalFilteringMode="StartsWith" />
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataComboBoxColumn VisibleIndex="2" EditFormSettings-VisibleIndex="0"
                        FieldName="Project!Key" Caption="Projekt" CellStyle-Wrap="False" Width="60px">
                        <PropertiesComboBox DataSourceID="ProjectsXpoData" TextField="ProjectName" ValueField="Oid"
                            IncrementalFilteringMode="StartsWith" />
                        <DataItemTemplate>
                            <div style="width: 100px; overflow: hidden; white-space: nowrap;" title='<%# GetProjectNameByOid(Eval("Project!Key")) %>'>
                                <%# GetProjectNameByOid(Eval("Project!Key"))%>
                            </div>
                        </DataItemTemplate>
                        <EditItemTemplate>
                            <dx:ASPxComboBox runat="server" ID="FavCmbProject" ClientInstanceName="favCmbProject"
                                ClientIDMode="Static" Width="100%" DropDownStyle="DropDownList" IncrementalFilteringMode="Contains"
                                Value='<%# Eval("[Project!Key]") %>' DataSourceID="ProjectsXpoData" TextFormatString="{0} ({1})"
                                ValueField="Oid" ValueType="System.Int32" EnableSynchronization="False">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="ProjectName" />
                                    <dx:ListBoxColumn FieldName="ProjectNumber" />
                                </Columns>
                                <ClientSideEvents SelectedIndexChanged="function(s, e) { OnFavProjectChanged(s); }"
                                    Init="onFavCmbEditProjectInit" />
                            </dx:ASPxComboBox>
                        </EditItemTemplate>
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataComboBoxColumn VisibleIndex="3" FieldName="PspCode!Key" Caption="PSP Code"
                        EditFormSettings-Visible="False" Visible="true">
                        <PropertiesComboBox DataSourceID="PspCodesXpoData" TextField="PspCodeNumber" ValueField="Oid"
                            IncrementalFilteringMode="StartsWith" />
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataComboBoxColumn VisibleIndex="4" EditFormSettings-VisibleIndex="2"
                        FieldName="PspCode!Key" Caption="PSP Bezeichnung" EditFormSettings-Caption="PSP Code"
                        CellStyle-Wrap="False">
                        <PropertiesComboBox DataSourceID="PspCodesXpoData" TextField="PspCodeName" ValueField="Oid"
                            IncrementalFilteringMode="StartsWith" />
                        <DataItemTemplate>
                            <div style="width: 100px; overflow: hidden; white-space: nowrap;" title='<%# GetPsPCodeNameByOid(Eval("Project!Key"), Eval("PspCode!Key")) %>'>
                                <%# GetPsPCodeNameByOid(Eval("Project!Key"), Eval("PspCode!Key")) %>
                            </div>
                        </DataItemTemplate>
                        <EditItemTemplate>
                            <dx:ASPxComboBox ID="FavCmbPspCode" ClientInstanceName="favCmbPsPCode" ClientIDMode="Static"
                                runat="server" Width="100%" DropDownStyle="DropDownList" Value='<%# Eval("[PspCode!Key]") %>'
                                DataSourceID="PspCodesXpoData" ValueField="Oid" ValueType="System.Int32"
                                TextFormatString="{0} ({1})" EnableCallbackMode="true" IncrementalFilteringMode="Contains"
                                CallbackPageSize="30" OnCallback="OnFavCmbPspCode_Callback">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="PspCodeName" />
                                    <dx:ListBoxColumn FieldName="PspCodeNumber" />
                                </Columns>
                            </dx:ASPxComboBox>
                        </EditItemTemplate>
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataComboBoxColumn VisibleIndex="5" FieldName="Activity!Key" Caption="Tätigkeits Nr"
                        EditFormSettings-Visible="False">
                        <PropertiesComboBox DataSourceID="ActivitiesXpoData" TextField="ActivityNumber" ValueField="Oid"
                            IncrementalFilteringMode="StartsWith" />
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataComboBoxColumn VisibleIndex="6" EditFormSettings-VisibleIndex="1"
                        FieldName="Activity!Key" Caption="Tätigkeit" CellStyle-Wrap="False" Width="60px">
                        <PropertiesComboBox DataSourceID="ActivitiesXpoData" TextField="ActivityName" ValueField="Oid"
                            IncrementalFilteringMode="StartsWith" />
                        <DataItemTemplate>
                            <div style="width: 70px; overflow: hidden; white-space: nowrap;" title='<%# GetActivityNameByOid(Eval("Project!Key"), Eval("Activity!Key")) %>'>
                                <%# GetActivityNameByOid(Eval("Project!Key"), Eval("Activity!Key"))%>
                            </div>
                        </DataItemTemplate>
                        <EditItemTemplate>
                            <dx:ASPxComboBox ID="FavCmbActivity" ClientInstanceName="favCmbActivity" runat="server" Width="100%" DropDownStyle="DropDown"
                                Value='<%# Eval("[Activity!Key]") %>' DataSourceID="ActivitiesXpoData" ValueField="Oid"
                                ValueType="System.Int32" TextFormatString="{0} ({1})" EnableCallbackMode="true"
                                IncrementalFilteringMode="Contains" CallbackPageSize="30" OnCallback="OnFavCmbActivity_Callback">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="ActivityName" />
                                    <dx:ListBoxColumn FieldName="ActivityNumber" />
                                </Columns>
                            </dx:ASPxComboBox>
                        </EditItemTemplate>
                    </dx:GridViewDataComboBoxColumn>
                    <dx:GridViewDataMemoColumn VisibleIndex="7" FieldName="Memo" Caption="Notiz" EditFormSettings-VisibleIndex="3"
                        CellStyle-Wrap="False">
                        <DataItemTemplate>
                            <div style="overflow: hidden; white-space: nowrap;" title="<%# Eval("[Memo]") %>">
                                <%# Eval("[Memo]") %>
                            </div>
                        </DataItemTemplate>
                        <EditItemTemplate>
                            <dx:ASPxTextBox ID="FavTbMemo" runat="server" Width="100%" Text='<%# Eval("[Memo]") %>' />
                        </EditItemTemplate>
                    </dx:GridViewDataMemoColumn>

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
                <ClientSideEvents CustomButtonClick="FavCustomButtonClicked" FocusedRowChanged="UpdateEditValuesByFavorites"
                    RowClick="FavRowClick" />
                <SettingsText CommandUpdate="Save" EmptyDataRow="noch keine Favoriten definiert" />
                <SettingsBehavior AllowFocusedRow="true" />
                <Styles AlternatingRow-Enabled="True" FocusedRow-BackColor="#0077C0" />
                <Settings ShowFooter="false" />
                <SettingsPager PageSize="50" Visible="true" />
            </dx:ASPxGridView>
        </div>
    </fieldset>
    <fieldset runat="server">
        <legend id="overviewAndFreeLegend" onclick="onOverviewAndFreeLegendClick(this)">Buchungsübersicht und Freigabe [-]</legend>
        <div id="overViewAndFreeContent" style="width: 100%">
            <div style="display: inline-flex; width: 23%; vertical-align: top; margin-top: 1px">
                <div>
                    <dx:ASPxCallbackPanel runat="server" ClientInstanceName="recordCalendarCallback">
                        <PanelCollection>
                            <dx:PanelContent>
                                <div style="display: block">
                                    <dx:ASPxCalendar ID="recordCalender" ClientInstanceName="recordCalender" ClientIDMode="Static"
                                        runat="server" ShowClearButton="false" ShowTodayButton="false" ShowWeekNumbers="true"
                                        EnableMultiSelect="true" ToolTip="Bitte wählen Sie das gewünschte Datum ihrer Buchung" AutoPostBack="false"
                                        TodayStyle-Border-BorderColor="#D61F01" DaySelectedStyle-BackColor="#0077C0"
                                        OnDayCellPrepared="recordCalender_DayCellPrepared">
                                        <ClientSideEvents SelectionChanged="function(s, e) { onCalendarSelectionChanged(s); }" />
                                        <DaySelectedStyle BackColor="#0077C0" />
                                        <TodayStyle>
                                            <Border BorderColor="#D61F01"></Border>
                                        </TodayStyle>
                                    </dx:ASPxCalendar>
                                </div>
                            </dx:PanelContent>
                        </PanelCollection>

                    </dx:ASPxCallbackPanel>
                    <div style="display: block">
                        <my:Legend runat="server" />
                    </div>
                    <dx:ASPxCallbackPanel runat="server" ClientInstanceName="summaryFieldCallback" OnCallback="OnSummaryFieldCallback">
                        <PanelCollection>
                            <dx:PanelContent>
                                <div style="display: block; border: solid; border-width: 1px; border-color: lightgray; margin-top: 25px; width: 220px; padding-top: 15px">
                                    <div style="display: block">
                                        <div class="summaryFieldStyle">
                                            <dx:ASPxLabel runat="server" Text="Ausgewählte Tage:" Visible="true" />
                                        </div>
                                        <dx:ASPxLabel runat="server" Text="0" ID="LblSelDays" />
                                    </div>
                                    <div style="display: block">
                                        <div class="summaryFieldStyle">
                                            <dx:ASPxLabel runat="server" Text="Anwesend:" Visible="true" />
                                        </div>
                                        <dx:ASPxLabel runat="server" Text="0 h" ID="LblSelAttendenceHours" />
                                    </div>
                                    <div style="display: block">
                                        <div class="summaryFieldStyle">
                                            <dx:ASPxLabel runat="server" Text="Gebucht:" Visible="true" />
                                        </div>
                                        <dx:ASPxLabel runat="server" Text="0 h" ID="LblSelBookedHours" />
                                    </div>
                                    <div style="display: block">
                                        <div class="summaryFieldStyle">
                                            <dx:ASPxLabel runat="server" Text="Differenz:" Visible="true" />
                                        </div>
                                        <dx:ASPxLabel runat="server" Text="0 h" ID="LblSelDiffHours" />
                                    </div>
                                    <div style="padding-top: 10px; padding-bottom: 10px;">
                                        <div style="display: block; padding-left: 50px; align-content: center">
                                            <dx:ASPxButton ID="BtnLock" runat="server" Text="Einträge freigeben" OnClick="OnBtnLockClick" HorizontalAlign="Center"
                                                Theme="Moderno" Paddings-Padding="2" Enabled="false" Font-Size="Small">
                                            </dx:ASPxButton>
                                        </div>
                                        <div style="display: block; padding-left: 30px; align-content: center">
                                            <dx:ASPxButton ID="BtnFree" runat="server" Text="Freigabe zurücknehmen" OnClick="OnBtnFreeClick" HorizontalAlign="Center"
                                                Theme="Moderno" Paddings-Padding="2" Enabled="false" Visible="false" Font-Size="Small">
                                            </dx:ASPxButton>
                                        </div>
                                    </div>

                                </div>
                            </dx:PanelContent>
                        </PanelCollection>
                    </dx:ASPxCallbackPanel>
                </div>
            </div>
            <div style="display: inline-flex; width: 74%">
                <asp:UpdatePanel runat="server" ID="HistoryGridUpdatePanel" UpdateMode="Conditional">
                    <ContentTemplate>
                        <dx:ASPxCallbackPanel runat="server" ClientInstanceName="historyGridCallback" ID="HistoryGridCallback">
                            <PanelCollection>
                                <dx:PanelContent>
                                    <my:HistoryGrid ID="InternalHistoryGrid" runat="server" ClientInstanceName="historyGrid"
                                        Visible="true" Editable="true" LeaderLockView="false" EmployeeLockView="false"></my:HistoryGrid>
                                </dx:PanelContent>
                            </PanelCollection>
                        </dx:ASPxCallbackPanel>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </fieldset>

    <dx:XpoDataSource ID="FavoritesXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Favorites">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="ProjectsXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Projects">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="ActivitiesXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Activities">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="PspCodesXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.PspCodes">
    </dx:XpoDataSource>
</asp:Content>
