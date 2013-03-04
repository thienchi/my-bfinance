<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewsc.ascx.cs" Inherits="dangdongcmm.ucnewsc" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server">    
    <div class="cname2" style="padding:5px 5px 5px 0px;">TRÍCH DẪN</div>
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <div class="newsquote">
                <div class="lname"><a href="/<%#Eval("UrlQuoute") %>"><%#Eval("Name")%></a></div>
                <div class="author"><%#Eval("Author")%></div>
            </div>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" PageSize="1" SortDir="Random" Visible="false" />
    
</ASP:PANEL>