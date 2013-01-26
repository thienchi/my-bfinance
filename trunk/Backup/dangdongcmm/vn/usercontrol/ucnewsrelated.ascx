<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewsrelated.ascx.cs" Inherits="dangdongcmm.ucnewsrelated" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="relatedl width100">
    <div class="title1bound">LIÊN QUAN</div>
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <div class="article" style="background:url(../images/divide1.png) no-repeat bottom left; padding-bottom:10px;">
                <div class="inlist"><a href="<%#Eval("eUrl") %>"><%#Eval("Name")%></a> <%#Eval("eIconex")%></div>
            </div>
        </ITEMTEMPLATE>
    </ASP:REPEATER>

</ASP:PANEL>
