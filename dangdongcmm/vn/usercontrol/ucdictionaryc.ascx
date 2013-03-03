<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewsc.ascx.cs" Inherits="dangdongcmm.ucnewsc" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="relatedl width100">
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <div class="newsdich">
                <div class="inlist">
                    <a href="/tu-dien-thuat-ngu-vn/<%#Eval("eUrl2") %>">
                    <%#(Eval("Name") + (Eval("Url").ToString() == "" ? "" : (" (" + Eval("Url") + ")")))%>
                   <%-- <a href="/tu-dien-thuat-ngu-vn/<%#Eval("eUrl2") %>"><%#Eval("Name")%></a>--%>
                    <%--<a href="d<%#Eval("Cid")%>d<%#Eval("Id")%>d=<%#Eval("eName")%>?ll=vn"><%#Eval("Name")%><%#Eval("Url").ToString() == "" ? "" : (" (" + Eval("Url") + ")")%>
                    </a>--%>
                </div>
            </div>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" Visible="false" PageSize="0" />
    
</ASP:PANEL>
<%--http://localhost:16168/d63d274d=Ngay-dang-ky-cuoi-cung.aspx?ll=vn
http://localhost:16168/danh-muc-vn/Ngay-dang-ky-cuoi-cung.aspx
http://localhost:16168/danh-muc-en/Ngay-dang-ky-cuoi-cung.aspx--%>