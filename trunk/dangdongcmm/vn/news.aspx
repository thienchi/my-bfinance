<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="news.aspx.cs" Inherits="dangdongcmm.news" %>
<%@ MASTERTYPE virtualpath="MasterDefault.Master" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="usercontrol/ucpager.ascx" %>
<%@ IMPORT Namespace="dangdongcmm.utilities" %>
<%@ REGISTER TagPrefix="UC" TagName="Cataloguemenu" Src="usercontrol/uccataloguemenu.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Comment" Src="usercontrol/uccomment.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Newsl" Src="usercontrol/ucnewsl.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Newsrelated" Src="usercontrol/ucnewsrelated.ascx" %>

<ASP:CONTENT ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript">
function EndRequestHandler(sender, args) {
    
}
</script>

    
                <ASP:PANEL ID="pnlCategory" runat="server">
                    <ASP:REPEATER ID="rptCategory" runat="server" >
                        <ITEMTEMPLATE></ITEMTEMPLATE>
                    </ASP:REPEATER>
                </ASP:PANEL>                    

                <ASP:PANEL ID="pnlInfo" runat="server" CssClass="relatedl width100">
                    <ASP:REPEATER ID="rptInfo" runat="server">
                        <ITEMTEMPLATE>

                            <ASP:HIDDENFIELD ID="Id" runat="server" Value='<%#Eval("Id") %>' />
                            <div class="article contentframe">
	                            <div class="DBRCCONTAINER">
                                    <div id="breadcrumbs"></div>
                                </div>
                                <div class="name"><%#Eval("Name")%> <%#Eval("eIconex")%></div>
	                            <div class="date">Đăng bởi: <%#Eval("Username").ToString() == "admin" ? "bFinance" : Eval("Username")%> | Ngày: <%#((DateTime)Eval("Timecreate")).ToString("dd.MM.yyyy, hh:mm tt")%></div>
                                <div class="sharebar">
                                    <div class="g">
                                        <div class="g-plus" data-action="share" data-annotation="bubble" data-href="http://bfinance.vn/<%#Eval("eUrl2")%>"></div>
                                        <script type="text/javascript">
                                           window.___gcfg = {        
                                            lang: 'en-US'      
                                           };
                                          (function() {
                                            var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
                                            po.src = 'https://apis.google.com/js/plusone.js';
                                            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);
                                          })();
                                        </script>
                                    </div>
                                    <script type="text/javascript" src="http://connect.facebook.net/en_US/all.js#xfbml=1"></script><fb:like href="http://bfinance.vn/<%#Eval("eUrl2")%>" layout="button_count" action="like" />
                                    
                                </div>           
                                <div class="desc"><%#Eval("eFilepreview")%> <%#Eval("Description")%></div>
                                <div align="right"><b><i><%#Eval("Author")%></i></b></div>
                                <div class="note"><%#CFunctions.IsNullOrEmpty(Eval("Tag").ToString()) ? "" : ("<b>Tag</b>: " + Eval("eTag2"))%></div>
                            </div>
                            <br />&nbsp;
                            <UC:COMMENT ID="Comment" runat="server" Allowcomment='<%#Eval("Allowcomment") %>' Belongto="12" Pid='<%#Eval("Id") %>' Pname='<%#Eval("Name") %>' />
                            
                            <UC:NEWSRELATED ID="Newsrelated" runat="server" Relateditem='<%#Eval("Relateditem")%>' />
                        </ITEMTEMPLATE>
                    </ASP:REPEATER>
                    
                </ASP:PANEL>
                
            <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
                <CONTENTTEMPLATE>
                
                <ASP:PANEL ID="pnlListfollow" runat="server" Visible="false" CssClass="relatedl width100">
                    <div class="title1bound">CÙNG DANH MỤC <ASP:LABEL ID="lblNamefollow" runat="server" Visible="false"></ASP:LABEL></div>
                    <ASP:REPEATER ID="rptListfollow" runat="server" >
                        <ITEMTEMPLATE>
                            <div class="article" style="background:url(../images/divide1.png) no-repeat bottom left; padding-bottom:10px;">
                                <%--<div class="inlist"><a href="<%#Eval("eUrl") %>?pag=<%=PageIndex %>"><%#Eval("Name")%></a> <%#Eval("eIconex")%></div>--%>
                                <div class="inlist"><a href="/<%#Eval("eUrl2") %>%>"><%#Eval("Name")%></a> <%#Eval("eIconex")%></div>
                            </div>
                        </ITEMTEMPLATE>
                    </ASP:REPEATER>
                    <div align="left" class="pagercontainer" style="text-align:left;">
                        <UC:PAGER ID="pagBuilderfollow" runat="server" CssClass="pagBuilder" Infotemplate="<b>$RECORDCOUNT$</b> bài viết" />
                    </div>
                </ASP:PANEL>

                <ASP:PANEL ID="pnlListByCategory" runat="server" CssClass="relatedl width100 contentframe">
                    <ASP:REPEATER ID="rptListByCategory" runat="server" OnItemDataBound="rptListByCategory_ItemDataBound">
                        <ITEMTEMPLATE>
                            <ASP:HIDDENFIELD ID="Id" runat="server" Value='<%#Eval("Id") %>' />
                            <div class="articleinmenu">
                                <div class="DBRCCONTAINER">
                                <a href="/">bFinance</a> <b>&nbsp;>>&nbsp;</b> 
                                <a href="/<%#Eval("eUrl2").ToString().Replace(".aspx","") + ".aspx" %>"><%#Eval("Name") %></a></div>
                                <UC:CATALOGUEMENU ID="Cataloguemenu" runat="server" Belongto="12" Cid='<%#Eval("Id") %>' />
                            </div>
                            <UC:NEWSL ID="Newsl" runat="server" />
                        </ITEMTEMPLATE>
                    </ASP:REPEATER>
                </ASP:PANEL>
                </CONTENTTEMPLATE>
            </ASP:UPDATEPANEL>
    
</ASP:CONTENT>
