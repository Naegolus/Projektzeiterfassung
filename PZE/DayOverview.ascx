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

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DayOverview.ascx.cs" Inherits="com.commend.tools.PZE.DayOverview" %>
<div style="display: block">
    <div class="summaryFieldStyle">
        <dx:ASPxLabel runat="server" Text="Zeitraum:" Visible="true" />
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