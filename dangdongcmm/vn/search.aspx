<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="search.aspx.cs" Inherits="dangdongcmm.search" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="usercontrol/ucpager.ascx" %>
<%@ IMPORT Namespace="dangdongcmm.utilities" %>

<ASP:CONTENT ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
            <ASP:PANEL ID="pnlList" runat="server" CssClass="relatedl width100" Style="border:1px solid #ccc; padding:10px; padding-top:0; width:629px">
                <div class="title1bound" style="margin-top:0;"><ASP:LABEL ID="lblCname" runat="server" Text="Kết quả tìm kiếm "></ASP:LABEL> bài viết</div>
                <div align="right" class="pagercontainer">
                    <UC:PAGER ID="pagBuilderT" runat="server" CssClass="pagBuilder" Infotemplate="<b>$RECORDCOUNT$</b> bài viết" />
                </div>
                <br />&nbsp;
                <div id="searchresult" class="relatedl width100">
                <ASP:REPEATER ID="rptList" runat="server">
                    <ITEMTEMPLATE>
                        <div class="article" style="margin-bottom:15px; padding-bottom:15px; border-bottom:1px solid #f2f2f2;">
                            <%#Eval("eFilepreview").ToString()==""?"":("<div class='blockimg'>" + Eval("eFilepreview") + "</div>")%>
                            <div class="lname"><a href="/<%#Eval("eUrl2") %>"><%#Eval("Name")%></a> <%#Eval("eIconex") %> <span class="date" style="float:none; top:0; width:auto;">&nbsp;(<%#Eval("Cname") %>)</span></div>
                            <%--<div class="intr"><%#Eval("Introduce") %> <a class="more" href="<%#Eval("eUrl2") %>?pag=<%=PageIndex %>">...chi tiết</a></div>--%>
                            <div class="intr"><%#Eval("Introduce") %> <a class="more" href="/<%#Eval("eUrl2") %>"> ...chi tiết</a></div>
                        </div>
                    </ITEMTEMPLATE>
                </ASP:REPEATER>
                </div>
                <div align="right" class="pagercontainer">
                    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="<b>$RECORDCOUNT$</b> bài viết" />
                </div>
            </ASP:PANEL>
            
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>

</ASP:CONTENT>