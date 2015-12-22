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

<%@ Control Language="C#" AutoEventWireup="true" Inherits="com.commend.tools.PZE.ColorLegendeEmployee" Codebehind="ColorLegendeEmployee.ascx.cs" %>

<asp:Table ID="Legend" runat="server">
    <asp:TableRow>
        <asp:TableCell>
            <dx:ASPxLabel runat="server" Width="10px" Height="10px" Border-BorderColor="Black" BackColor="LightGray" />
        </asp:TableCell>
        <asp:TableCell>
            <dx:ASPxLabel runat="server" Text="Unterbucht" />
        </asp:TableCell>
        <asp:TableCell>
            <dx:ASPxLabel runat="server" Width="10px" Height="10px" Border-BorderColor="Black" BackColor="Salmon" />
        </asp:TableCell>
        <asp:TableCell>
            <dx:ASPxLabel runat="server" Text="Überbucht" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            <dx:ASPxLabel runat="server" Width="10px" Height="10px" Border-BorderColor="Black" BackColor="LightGreen" />
        </asp:TableCell>
        <asp:TableCell>
            <dx:ASPxLabel runat="server" Text="Ausreichend Zeit gebucht" />
        </asp:TableCell>
        <asp:TableCell>
            <dx:ASPxLabel runat="server" Width="10px" Height="10px" Border-BorderColor="Black" BackColor="DarkSeaGreen" />
        </asp:TableCell>
        <asp:TableCell>
            <dx:ASPxLabel runat="server" Text="Tag freigegeben" />
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>