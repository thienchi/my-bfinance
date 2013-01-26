<%@ PAGE Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="categoryattrl.aspx.cs" Inherits="dangdongcmm.cmm.categoryattrl" %>
<%@ MASTERTYPE virtualpath="MasterDefault.master" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="usercontrol/ucpager.ascx" %>
<%@ REGISTER TagPrefix="UC" TagName="Setupattribute" Src="usercontrol/ucsetupattribute.ascx" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        
        <ASP:LABEL ID="lblPath" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ASP:PANEL id="pnlList" runat="server" width="100%">
            <div class="dformoutcommand">
                <div class="colfilter">
                    <ASP:PANEL ID="panelSearch" runat="server" DefaultButton="cmdSearch">
                        <table cellspacing="0" cellpadding="0" border="0">
                            <tr>
                                <td><img src="/cmm/images/symsearch.gif" alt="" /></td>
                                <td><ASP:DROPDOWNLIST ID="ddlCid" runat="server" onchange="ddlGotopage(this, 'categoryattrl.aspx');"></ASP:DROPDOWNLIST></td>
                                <td><ASP:TEXTBOX ID="txtKeyword" runat="server"></ASP:TEXTBOX></td>
                                <td><ASP:BUTTON ID="cmdSearch" runat="server" OnClientClick="javascript:return doSubmit();" OnClick="cmdSearch_Click" CssClass="button" Text="Search" /></td>
                            </tr>
                        </table>
                    </ASP:PANEL>
                </div>
                <div class="colsetting">
                    <ASP:BUTTON ID="cmdAdd" runat="server" Text="Thêm mới" CssClass="buttoncmd" />&nbsp;
                    -- hoặc --
                    <ASP:DROPDOWNLIST ID="ddlAction" runat="server" onchange="actionMulti(this);" OnSelectedIndexChanged="ddlAction_SelectedIndexChanged">
                        <ASP:LISTITEM Value="" class="textdefndis">Thiết lập cho nhiều danh mục...</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Delete">-- Xóa</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Copy">-- Sao chép</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Move">-- Di chuyển</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Updatedisplayorder">-- Cập nhật thứ tự hiển thị</ASP:LISTITEM>
                        <ASP:LISTITEM Value="Setupattribute">-- Cài đặt hiển thị</ASP:LISTITEM>
                    </ASP:DROPDOWNLIST>
                </div>  
            </div>
            
            <div class="dformoutlist">
            <ASP:GRIDVIEW ID="grdView" runat="server" DataKeyNames="Id" CssClass="lgridview" 
                AutoGenerateColumns="false" AllowSorting="true" AllowPaging="false" OnRowDeleting="grdView_RowDeleting" OnRowCommand="grdView_RowCommand"
                OnRowDataBound="grdView_RowDataBound" OnSorting="grdView_Sorting">
                <COLUMNS>
                    <ASP:TEMPLATEFIELD HeaderText="#" HeaderStyle-CssClass="colnum" ItemStyle-CssClass="colnum">
                        <ITEMTEMPLATE>
                            <%# Eval("Rownumber")%>
                            <ASP:HIDDENFIELD ID="txtId" runat="server" Value='<%#Eval("Id") %>' />
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD itemstyle-cssclass="colcheck">
                        <HEADERTEMPLATE>
                            <input type="checkbox" id="chkcheckall" onclick="tglSel('chkcheckall', 'chkcheck');" />
                        </HEADERTEMPLATE>
                        <ITEMTEMPLATE>
                            <input type="checkbox" id="chkcheck<%# Eval("Id")%>" name="chkcheck" value="<%# Eval("Id")%>" onclick="chkAll('chkcheckall', this);" />
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Tên/ Tiêu đề" sortexpression="Name">
                        <ITEMTEMPLATE>
                            <%# Eval("eMarkas")%> <ASP:LINKBUTTON ID="cmdSetupattribute" runat="server" CommandName="Setupattribute" CommandArgument='<%# Eval("Id")%>' Text='<%#Eval("Name") %>' CausesValidation="false"></ASP:LINKBUTTON> <%#Eval("Iconex").ToString() == "" ? "" : "<img src='../../" + Eval("Iconex") + "' />" %>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Danh mục con" sortexpression="Depth" itemstyle-cssclass="colpis">
                        <ITEMTEMPLATE>
                            <a href='categoryattrl.aspx?cid=<%#Eval("Cid") %>&pid=<%#Eval("Id") %>'><%#Eval("Pis").ToString() == "0" ? "" : ("Danh mục con " + (Convert.ToInt32(Eval("Depth")) + 1)) + " (" + Eval("ePis") + ")"%></a>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Thứ tự" sortexpression="Orderd" itemstyle-cssclass="colorderd">
                        <ITEMTEMPLATE>
                            <ASP:TEXTBOX ID="txtOrderd" runat="server" CssClass="ingrdinput" Text='<%#Eval("Orderd") %>' onkeypress="javascript:return UINumber_In(event);" onblur="UINumber_Out(this)"></ASP:TEXTBOX>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:BOUNDFIELD datafield="eStatus" headertext="Trạng thái" sortexpression="Status" itemstyle-cssclass="colstatus" HtmlEncode="false"></ASP:BOUNDFIELD>
                    <ASP:TEMPLATEFIELD itemstyle-cssclass="colaction">
                        <ITEMTEMPLATE>
                            <a class="command" href="javascript:CC_gotoUrl('categoryattru.aspx?cid=<%#Eval("Cid") %>&pag=<%=PageIndex %>&pid=<%#Eval("Pid") %>&iid=<%#Eval("Id") %>');">Sửa</a>
                            &nbsp;|&nbsp;
                            <ASP:LINKBUTTON ID="cmdDeleteone" runat="server" CssClass="command" Text="Xóa" CommandName="Delete" CommandArgument='<%# Eval("Id")%>' CausesValidation="false"></ASP:LINKBUTTON>
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
            <UC:SETUPATTRIBUTE ID="Setupattribute" runat="server" />
        </ASP:PANEL>

        <ASP:HIDDENFIELD ID="txtIidstr" runat="server" />
        <ASP:PANEL id="pnlCopy" runat="server" Visible="false" CssClass="tformin tformincom">
            <div class="rowhor"><b>Các thông tin sau sẽ được sao chép:</b></div>
            <div class="rowhor">
                <ASP:GRIDVIEW ID="grdCopy" runat="server" CssClass="lgridnoborder"
                    AutoGenerateColumns="false" AllowSorting="false" ShowHeader="false">
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
            <div class="rowhor"><b>Sao chép qua:</b></div>
            <div class="rowhor">
                <div class="boxctrlc">Loại danh mục</div>
                <div class="boxctrlc" style="margin-left:20px;">Danh mục gốc</div>
            </div>
            <div class="rowhor">
                <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlCopycid" runat="server" AutoPostBack="true" onchange="javascript:doChange();" OnSelectedIndexChanged="ddlCopycid_SelectedIndexChanged"></ASP:DROPDOWNLIST></div>
                <div class="boxctrlc" style="margin-left:20px;"><ASP:DROPDOWNLIST ID="ddlCopypid" runat="server"></ASP:DROPDOWNLIST></div>
            </div>
            <br />&nbsp;
            <div class="rowhor">
                <fieldset>
                    <legend>
                        <ASP:BUTTON ID="cmdCopyOk" runat="server" CssClass="button" Text="Sao chép" OnClientClick="javascript:return doSubmit();" OnClick="cmdCopyOk_Click" Enabled="false" />
                        <input type="button" class="button" value="Quay lại" onclick="javascript:location.href=location.href;" />
                    </legend>
                    <div>
                        <ASP:CHECKBOX ID="chkCopyoption_getchild" runat="server" Text="Sao chép đồng thời tất cả danh mục con?" />
                    </div>    
                </fieldset>
            </div>
        </ASP:PANEL>

        <ASP:PANEL id="pnlMove" runat="server" Visible="false" CssClass="tformin tformincom">
            <div class="rowhor"><b>Các thông tin sau sẽ được di chuyển:</b></div>
            <div class="rowhor">
                <ASP:GRIDVIEW ID="grdMove" runat="server" CssClass="lgridnoborder"
                    AutoGenerateColumns="false" AllowSorting="false" ShowHeader="false">
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
                <div class="boxctrlc">Loại danh mục</div>
                <div class="boxctrlc" style="margin-left:20px;">Danh mục gốc</div>
            </div>
            <div class="rowhor">
                <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlMovecid" runat="server" AutoPostBack="true" onchange="javascript:doChange();" OnSelectedIndexChanged="ddlMovecid_SelectedIndexChanged"></ASP:DROPDOWNLIST></div>
                <div class="boxctrlc" style="margin-left:20px;"><ASP:DROPDOWNLIST ID="ddlMovepid" runat="server"></ASP:DROPDOWNLIST></div>
            </div>
            <br />&nbsp;
            <div class="rowhor">
                <fieldset>
                    <legend>
                        <ASP:BUTTON ID="cmdMoveOk" runat="server" CssClass="button" Text="Di chuyển" OnClientClick="javascript:return doSubmit();" OnClick="cmdMoveOk_Click" Enabled="false" />
                        <input type="button" class="button" value="Quay lại" onclick="javascript:location.href=location.href;" />
                    </legend>
                    <div>
                        <ASP:CHECKBOX ID="chkMoveoption_getchild" runat="server" Checked="true" Text="Di chuyển đồng thời tất cả danh mục con?" />
                    </div>    
                </fieldset>
            </div>
        </ASP:PANEL>
        
        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    
</ASP:CONTENT>
