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

<%@ Page  Title="Vorgesetzten Freigabe" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="com.commend.tools.PZE.LockRecordLeader" Codebehind="LockRecordLeader.aspx.cs" %>

<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Xpo.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Xpo" TagPrefix="dx" %>

<%@ Register TagPrefix="my" TagName="HistoryGrid" Src="~/HistoryGrid.ascx" %>
<%@ Register TagPrefix="my" TagName="Legend" Src="~/ColorLegendeLeaderLock.ascx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script src="Scripts/shortcut.js" type="text/javascript"></script>
    <script type="text/javascript">

        function OnLockRecordsCalendarSelectionChanged(lockRecordsCalender) {
            notifyCalendarSelChangedGrid.PerformCallback();
            notifyCalendarSelChangedSum.PerformCallback();
        }

    </script>
    <fieldset runat="server">
        <legend>Vorgesetztenfreigabe</legend>
        <table>
            <tr align="center">
                <td align="center">
                    <asp:Table runat="server">
                        <asp:TableRow>
                            <asp:TableCell>
                                <dx:ASPxCalendar ID="LockRecordsCalendar" ClientInstanceName="lockRecordsCalender" ClientIDMode="Static"
                                    runat="server" ShowClearButton="false" ShowTodayButton="false" ShowWeekNumbers="true"
                                    EnableMultiSelect="true" ToolTip="Bitte wählen Sie das gewünschte Datum ihrer Buchung" AutoPostBack="false"
                                    TodayStyle-Border-BorderColor="#D61F01" DaySelectedStyle-BackColor="#0077C0" 
                                    OnDayCellPrepared="OnLockRecordsCalendar_DayCellPrepared">
                                    <ClientSideEvents SelectionChanged="function(s, e) { OnLockRecordsCalendarSelectionChanged(s); }" />
                                    <DaySelectedStyle BackColor="#0077C0">
                                    </DaySelectedStyle>
                                    <TodayStyle>
                                        <Border BorderColor="#D61F01"></Border>
                                    </TodayStyle>
                                </dx:ASPxCalendar>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell HorizontalAlign="Center">
                                <my:Legend runat="server"/>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </td>
                <td style="width:30px"/>
                <td style="vertical-align:top">
                    <fieldset>
                        <table style="width: 500px">
                            <tr>
                                <td align="left">
                                    <dx:ASPxLabel runat="server" Text="Mitarbeiter " Visible="true">
                                    </dx:ASPxLabel>
                                </td>
                                <td style="white-space: nowrap" colspan="3">
                                    <dx:ASPxComboBox runat="server" ID="CmbUser" ClientInstanceName="cmbUser" ClientIDMode="Static" 
                                        DropDownStyle="DropDownList" TextFormatString="{0} {1} ({2})" OnInit="OnCmbUserInit"
                                        ValueField="Oid" ValueType="System.Int32" IncrementalFilteringMode="Contains"
                                        EnableSynchronization="False" Width="300px" Visible="true" AutoPostBack="true"
                                        OnSelectedIndexChanged="CmbUser_IndexChanged">
                                        <Columns>
                                            <dx:ListBoxColumn FieldName="ForName" Caption="Vorname" Width="150px"/>
                                            <dx:ListBoxColumn FieldName="SurName" Caption="Nachname" Width="150px" />
                                            <dx:ListBoxColumn FieldName="PersonalId" Caption="Personal Nr" Width="70px"/>
                                        </Columns>
                                        <ClientSideEvents SelectedIndexChanged="function(s, e) { OnLockRecordsCalendarSelectionChanged(s); }" />
                                    </dx:ASPxComboBox>
                                </td>
                            </tr>
                        </table>
                        <dx:ASPxCallbackPanel runat="server" ClientInstanceName="notifyCalendarSelChangedSum"
                             OnCallback="NotifyCalendarSelChangedSum_Callback" RenderMode="Table">
                            <PanelCollection>
                                <dx:PanelContent>
                                    <table style="height: 170px; width: 450px">
                                        <tr>
                                            <td align="left">
                                                <dx:ASPxLabel runat="server" Text="Ausgewählte Tage: " Visible="true" />
                                            </td>
                                            <td style="white-space: nowrap; padding-left: 3px;" colspan="3">
                                               <dx:ASPxLabel runat="server" Text="0" ID="LblSelDays"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <dx:ASPxLabel runat="server" Text="Anwesend:" Visible="true" />
                                            </td>
                                            <td style="white-space: nowrap; padding-left: 3px;" colspan="3">
                                               <dx:ASPxLabel runat="server" Text="0 h" ID="LblSelAttendenceHours"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <dx:ASPxLabel runat="server" Text="Gebucht:" Visible="true" />
                                            </td>
                                            <td style="white-space: nowrap; padding-left: 3px;" colspan="3"">
                                               <dx:ASPxLabel runat="server" Text="0 h" ID="LblSelBookedHours"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left">
                                                <dx:ASPxLabel runat="server" Text="Differenz:" Visible="true" />
                                            </td>
                                            <td style="white-space: nowrap; padding-left: 3px;" colspan="3">
                                               <dx:ASPxLabel runat="server" Text="0 h" ID="LblSelDiffHours"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td align="left" valign="top">
                                                <dx:ASPxLabel runat="server" Text="Kommentar:" Visible="true" />
                                            </td>
                                            <td style="white-space: nowrap; padding-left: 3px" colspan="3">
                                               <dx:ASPxLabel runat="server" Width="300px" Height="75px" Wrap="True" ID="LblSelComments"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <dx:ASPxButton ID="BtnLock" Enabled="false" runat="server" Text="Einträge freigeben" OnClick="BtnLock_Click" />
                                            </td>
                                            <td>
                                                <dx:ASPxButton ID="BtnFree" Enabled="false" runat="server" Text="Freigaben zurücknehmen" OnClick="BtnFree_Click" />
                                            </td>
                                        </tr>
                                    </table>                                    
                                </dx:PanelContent>
                            </PanelCollection>
                        </dx:ASPxCallbackPanel>
                    </fieldset>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <dx:ASPxCallbackPanel runat="server" ClientInstanceName="notifyCalendarSelChangedGrid"
                    OnCallback="NotifyCalendarSelChangedGrid_Callback" RenderMode="Table">
                    <PanelCollection>
                        <dx:PanelContent runat="server">
                            <my:HistoryGrid ID="InternalHistoryGrid" runat="server" Visible="true" LeaderLockView="true" EmployeeLockView="false"/>
                        </dx:PanelContent>
                    </PanelCollection>
                </dx:ASPxCallbackPanel>
            </tr>
        </table>
    </fieldset>
</asp:Content>