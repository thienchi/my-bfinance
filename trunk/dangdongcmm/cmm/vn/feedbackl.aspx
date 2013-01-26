<%@ PAGE Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="feedbackl.aspx.cs" Inherits="dangdongcmm.cmm.feedbackl" %>
<%@ MASTERTYPE virtualpath="MasterDefault.master" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="usercontrol/ucpager.ascx" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ASP:PANEL id="pnlList" runat="server" width="100%">
            <div class="dformoutcommand">
                <div class="colfilter">
                    <ASP:PANEL ID="panelSearch" runat="server" DefaultButton="cmdSearch">
                        <table cellspacing="0" cellpadding="0" border="0">
                            <tr>
                                <td><img src="../images/symsearch.gif" alt="" /></td>
                                <td><ASP:TEXTBOX ID="txtKeyword" runat="server"></ASP:TEXTBOX></td>
                                <td><ASP:BUTTON ID="cmdSearch" runat="server" OnClientClick="javascript:return doSubmit();" OnClick="cmdSearch_Click" CssClass="button" Text="Search" /></td>
                            </tr>
                        </table>
                    </ASP:PANEL>
                </div>
                <div class="colsetting">
                    <table cellspacing="0" cellpadding="0" border="0" align="right">
                        <tr>
                            <td>
                    -- hoặc --
                    <ASP:DROPDOWNLIST ID="ddlAction" runat="server" onchange="actionMulti(this);" OnSelectedIndexChanged="ddlAction_SelectedIndexChanged">
                        <ASP:LISTITEM Value="" class="textdefndis">Thiết lập khác...</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Delete">-- Xóa</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Move">-- Di chuyển</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Updatedisplayorder">-- Create a Folder</ASP:LISTITEM>
                    </ASP:DROPDOWNLIST>
                            </td>
                            <td>&nbsp; <ASP:TEXTBOX ID="txtFolderName" runat="server" Visible="false"></ASP:TEXTBOX></td>
                            <td>&nbsp; <ASP:BUTTON ID="cmdCreateFolder" runat="server" OnClientClick="javascript:return doSubmit();" OnClick="cmdCreateFolder_Click" CssClass="button" Text="Create Folder" Visible="false" /></td>
                        </tr>
                    </table>    
                </div>
            </div>
            
            <div class="dformoutlist">
            <ASP:GRIDVIEW ID="grdView" runat="server" DataKeyNames="Id" CssClass="lgridview" 
                AutoGenerateColumns="false" AllowSorting="true" AllowPaging="false" 
                OnRowDataBound="grdView_RowDataBound" OnSorting="grdView_Sorting" OnRowDeleting="grdView_RowDeleting" OnRowCommand="grdView_RowCommand" >
                <COLUMNS>
                    <ASP:TEMPLATEFIELD itemstyle-cssclass="colcheck" ItemStyle-VerticalAlign="Top">
                        <HEADERTEMPLATE>
                            <input type="checkbox" id="chkcheckall" onclick="tglSel('chkcheckall', 'chkcheck');" />
                        </HEADERTEMPLATE>
                        <ITEMTEMPLATE>
                            <ASP:HIDDENFIELD ID="txtId" runat="server" Value='<%#Eval("Id") %>' />
                            <input type="checkbox" id="chkcheck<%# Eval("Id")%>" name="chkcheck" value="<%# Eval("Id")%>" onclick="chkAll('chkcheckall', this);" />
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Tên/ Tiêu đề" sortexpression="Name">
                        <ITEMTEMPLATE>
                            <ASP:LINKBUTTON ID="cmdViewinfo" runat="server" CommandName="Viewinfo" CommandArgument='<%# Eval("Id")%>' CausesValidation="false"></ASP:LINKBUTTON>
                            <%#int.Parse(Eval("Pis").ToString()) > 0 ? "<img src='../images/icon_folder.gif' align='absmiddle' />" : "" %> <%#int.Parse(Eval("Pis").ToString()) > 0 ? ("<b>" + Eval("Name") + "</b>") : ("<a href=javascript:showContent('dc" + Eval("Id") + "','" + (Container.DataItemIndex + 2) + "');>" + (Eval("Viewcounter").ToString() == "0" ? ("<b>" + Eval("Name") + "</b>") : Eval("Name")) + "</a>") %>
                            <div id="dc<%#Eval("Id") %>" style="display:none">
                                <p style="color:#555">
                                Từ: <%#Eval("Sender_Name") %><br />
                                E-mail: <a href="mailto:<%#Eval("Sender_Email") %>"><%#Eval("Sender_Email")%></a>
                                Điện thoại: <%#Eval("Sender_Phone")%><br />
                                Địa chỉ: <%#Eval("Sender_Address")%><br />
                                </p>
                                <%#Eval("Description") %>
                            </div>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="" sortexpression="Depth" itemstyle-cssclass="colpis">
                        <ITEMTEMPLATE>
                            <a href='feedbackl.aspx?pid=<%#Eval("Id") %>'><%#Eval("Pis").ToString() == "0" ? "" : ("<b>Thư (" + Eval("ePis") + ")</b>") %></a>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Người gửi" sortexpression="Sender_Name" itemstyle-cssclass="colinfo3">
                        <ITEMTEMPLATE>
                            <div><%#int.Parse(Eval("Pis").ToString()) > 0 ? ("<b>" + Eval("Sender_Name") + "</b>") : Eval("Sender_Email")%></div>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Gửi lúc" sortexpression="Timeupdate" itemstyle-cssclass="colinfo1">
                        <ITEMTEMPLATE>
                            <div><%#int.Parse(Eval("Pis").ToString()) > 0 ? ("<b>" + Eval("eTimeupdateshort") + "</b>") : Eval("eTimeupdateshort")%></div>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD itemstyle-cssclass="colaction">
                        <ITEMTEMPLATE>
                            &nbsp;|&nbsp;
                            <ASP:LINKBUTTON ID="cmdDeleteone" runat="server" CssClass="command" Text="Xóa" CommandName="Delete" CausesValidation="false"></ASP:LINKBUTTON>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                </COLUMNS>
                <HEADERSTYLE cssclass="rowheader" />
                <ROWSTYLE cssclass="rowormal" />
                <ALTERNATINGROWSTYLE cssclass="rowalter" />
                
            </ASP:GRIDVIEW>
            <div align="right" class="pagercontainer">
		        <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" />
            </div>
            </div>
        </ASP:PANEL>
        <script type="text/javascript">
        function showContent(dc, index){
            $("#"+dc).toggle();
            
            if($("#"+dc).is(":hidden")){
                __doPostBack("ctl00$ContentPlaceHolder1$grdView$ctl" + (index > 10 ? index : ("0" + index)) + "$cmdViewinfo","");
            }    
        }
        </script>

        <ASP:HIDDENFIELD ID="txtIidstr" runat="server" />
        <ASP:PANEL id="pnlMove" runat="server" Visible="false" CssClass="tformin tformincom">
            <div class="rowhor"><b>Các thông tin sau sẽ được di chuyển:</b></div>
            <div class="rowhor">
                <ASP:GRIDVIEW id="grdMove" runat="server" cssclass="lgridnoborder"
                    autogeneratecolumns="false" allowsorting="true" ShowHeader="false">
                    <COLUMNS>
                        <ASP:TEMPLATEFIELD ItemStyle-CssClass="colstatus">
                            <ITEMTEMPLATE>
                                <li>[ <%#Eval("Id") %> ] &nbsp; 
                            </ITEMTEMPLATE>
                        </ASP:TEMPLATEFIELD>
                        <ASP:TEMPLATEFIELD>
                            <ITEMTEMPLATE>
                                <a><%#Eval("Name") %></a>
                            </ITEMTEMPLATE>
                        </ASP:TEMPLATEFIELD>
                    </COLUMNS>
                    <ROWSTYLE CssClass="rowormal" />
                </ASP:GRIDVIEW>
            </div>
            <div class="rowhor"><b>Di chuyển qua:</b></div>
            <div class="rowhor">
                <div class="boxctrlc">Thư mục</div>
            </div>
            <div class="rowhor">
                <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlMovepid" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlMovepid_SelectedIndexChanged"></ASP:DROPDOWNLIST></div>
            </div>
            <br />&nbsp;
            <div class="rowhor">
                <fieldset>
                    <legend>
                        <ASP:BUTTON ID="cmdMoveOk" runat="server" CssClass="button" Text="Di chuyển" OnClientClick="javascript:return doSubmit();" OnClick="cmdMoveOk_Click" Enabled="false" />
                        <input type="button" class="button" value="Quay lại" onclick="javascript:location.href=location.href;" />
                    </legend>
                    <div>
                        <ASP:CHECKBOX ID="chkMoveoption_getchild" runat="server" Checked="true" Text="Di chuyển đồng thời tất cả thư trong thư mục?" />
                    </div>    
                </fieldset>
            </div>
        </ASP:PANEL>
        
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    
</ASP:CONTENT>
