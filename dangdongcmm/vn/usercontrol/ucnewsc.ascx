﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewsc.ascx.cs" Inherits="dangdongcmm.ucnewsc" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="fieldsetf" Style="margin-top:15px;">
    <h4>TRÍCH DẪN</h4>
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <div class="newsquote">
                <div class="lname"><a href="/<%#Eval("eUrl2") %>"><%#Eval("Name")%></a></div>
                <div class="author"><%#Eval("Author")%></div>
            </div>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" PageSize="1" SortDir="Random" Visible="false" />
    
</ASP:PANEL>