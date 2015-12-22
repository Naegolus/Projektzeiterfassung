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

<%@ Page Title="Access Denied" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="com.commend.tools.PZE.Account.AccessDenied" Codebehind="AccessDenied.aspx.cs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div align="center">
        <h2>
            Access Denied!
        </h2>
        <p>
            Sie besitzen nicht die entsprechende Berechtigung, um auf diese Seite zugreifen
            zu dürfen!
        </p>
    </div>
</asp:Content>
