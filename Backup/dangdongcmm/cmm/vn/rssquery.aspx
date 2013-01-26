<%@ PAGE Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="rssquery.aspx.cs" Inherits="dangdongcmm.cmm.rssquery" %>
<%@ MASTERTYPE virtualpath="MasterDefault.Master" %>
<%@ REGISTER TagPrefix="RSS" Namespace="RssToolkit" Assembly="RssToolkit, Version=1.0.0.1, Culture=neutral, PublicKeyToken=02e47a85b237026a" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ASP:PANEL id="pnlList" runat="server" width="100%">
            <div class="dformoutcommand">
                <div class="colfilter">
                    <ASP:PANEL ID="panelSearch" runat="server" DefaultButton="cmdQuery">
                        <table cellspacing="0" cellpadding="0" border="0">
                            <tr>
                                <td><img src="../images/symsearch.gif" alt="" /></td>
                                <td><ASP:DROPDOWNLIST ID="ddlRSSUrl" runat="server" onchange="getRSSUrl();"></ASP:DROPDOWNLIST></td>
                                <td><ASP:TEXTBOX ID="txtRSSUrl" runat="server" Width="200px"></ASP:TEXTBOX></td>
                                <td><ASP:BUTTON ID="cmdQuery" runat="server" OnClientClick="javascript:return doSubmit();" OnClick="cmdQuery_Click" CssClass="button" Text="Truy vấn tin" /></td>
                            </tr>
                        </table>
                        <script type="text/javascript">
                        function getRSSUrl() {
                            var ddl = VALIDATA.GetObj('<%=ddlRSSUrl.ClientID %>');
                            var txt = VALIDATA.GetObj('<%=txtRSSUrl.ClientID %>');
                            txt.val(VALIDATA.GetVal(ddl));
                        }
                        </script>
                    </ASP:PANEL>
                </div>
            </div>

            <div class="tformin tformincom" style="font-size:11px;">
                <ASP:REPEATER ID="rptList" runat="server">
                    <ITEMTEMPLATE>
                        <ASP:HIDDENFIELD ID="txtId" runat="server" Value='<%#Eval("Id") %>' />
                        <ASP:HIDDENFIELD ID="txtWebsiteUrl" runat="server" Value='<%#Eval("WebsiteUrl") %>' />
                        <ASP:HIDDENFIELD ID="txtNodecontent" runat="server" Value='<%#Eval("Nodecontent") %>' />
                        <ASP:HIDDENFIELD ID="txtNodetitle" runat="server" Value='<%#Eval("Nodetitle") %>' />
                        <ASP:HIDDENFIELD ID="txtNodeintroduce" runat="server" Value='<%#Eval("Nodeintroduce") %>' />
                        <div>Nguồn tin: <%#Eval("Name") %></div>
                        <div>Lần lấy tin cuối cùng: <%#Eval("eTimelastestget") %></div>
                        <div>Lưu tin vào danh mục: <%#Eval("Cname") %></div>
                    </ITEMTEMPLATE>
                </ASP:REPEATER>            
            </div>
            <div class="tformin tformincom">
                <div class="rowhor">
                    <div class="boxctrll"><i>Đánh dấu lựa chọn các tin muốn lưu vào hệ thống và lựa chọn danh mục để lưu vào</i></div>
                    <div class="boxctrll">
                        <ASP:RADIOBUTTONLIST ID="radSavetype" runat="server" RepeatDirection="Horizontal">
                            <ASP:LISTITEM Selected="True" Value="0">Lưu toàn bộ nội dung tin vào hệ thống</ASP:LISTITEM>
                            <ASP:LISTITEM Value="1">Chỉ tham khảo đến trang nguồn</ASP:LISTITEM>
                        </ASP:RADIOBUTTONLIST></div>
                </div>
                <div class="rowhor">
                    <div class="relative"><i>Lưu tin vào danh mục:</i></div>
                    <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlCid" runat="server"></ASP:DROPDOWNLIST></div>
                    <div class="boxctrls" style="margin-left:15px;"><ASP:BUTTON ID="cmdSave" runat="server" CssClass="buttonin" Text="Lấy tin & lưu" OnClientClick="javascript:return doSubmitSel();" OnClick="cmdSave_Click" /></div>
                </div>
                <br />&nbsp;<br />&nbsp;
            </div>
            
            <div class="dformoutlist">
            <RSS:RSSDATASOURCE id="RssDataSource1" runat="server" maxitems="0" ></RSS:RSSDATASOURCE>
            <ASP:GRIDVIEW ID="grdView" runat="server" CssClass="lgridview" DataSourceID="RssDataSource1"
                AutoGenerateColumns="false" AllowSorting="false" AllowPaging="false" 
                OnRowDataBound="grdView_RowDataBound">
                <COLUMNS>
                    <ASP:TEMPLATEFIELD HeaderText="#" HeaderStyle-CssClass="colnum" ItemStyle-CssClass="colnum">
                        <ITEMTEMPLATE>
                            <%# Container.DataItemIndex+1 %>
                            <ASP:HIDDENFIELD ID="txtId" runat="server" Value='<%# Container.DataItemIndex %>' />
                            <ASP:HIDDENFIELD ID="txtTitle" runat="server" Value='<%# Eval("title") %>' />
                            <ASP:HIDDENFIELD ID="txtDescription" runat="server" Value='<%# Eval("description") %>' />
                            <ASP:HIDDENFIELD ID="txtLink" runat="server" Value='<%# Eval("link") %>' />
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD itemstyle-cssclass="colcheck">
                        <HEADERTEMPLATE>
                            <input type="checkbox" id="chkcheckall" onclick="tglSel('chkcheckall', 'chkcheck');" />
                        </HEADERTEMPLATE>
                        <ITEMTEMPLATE>
                            <input type="checkbox" id="chkcheck<%# Container.DataItemIndex %>" name="chkcheck" value="<%# Container.DataItemIndex %>" onclick="chkAll('chkcheckall', this);" />
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Tên / Tiêu đề">
                        <ITEMTEMPLATE>
                            <div><a target="_blank" href="<%# Eval("link") %>"><b><%# Eval("title") %></b></a></div>
                            <div class="rsscontent"><%# Eval("description") %></div>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Xuất bản" itemstyle-cssclass="colinfo3">
                        <ITEMTEMPLATE>
                            <div style="color:#555;"><%#Eval("pubDate") %></div>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    
                </COLUMNS>
                <HEADERSTYLE cssclass="rowheader" />
                <ROWSTYLE cssclass="rowormal" />
                <ALTERNATINGROWSTYLE cssclass="rowalter" />
                
            </ASP:GRIDVIEW>
            </div>
        </ASP:PANEL>
        
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    
</ASP:CONTENT>
