<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewsin.ascx.cs" Inherits="dangdongcmm.ucnewsin" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server">
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <div class="newshomer"><div class="linklist"><a href="/<%#Eval("eUrl2") %>"><%#Eval("Name") %></a></div></div>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    <ASP:HIDDENFIELD ID="hidMarkas" runat="server" Value="1" />
    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="<b>$RECORDCOUNT$</b> bài viết" Visible="false" />
</ASP:PANEL>