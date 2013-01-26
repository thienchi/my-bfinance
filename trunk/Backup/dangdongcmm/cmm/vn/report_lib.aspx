<%@ PAGE Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="report_lib.aspx.cs" Inherits="dangdongcmm.cmm.report_lib" %>
<%@ MASTERTYPE virtualpath="MasterDefault.master" %>
<%@ REGISTER TagPrefix="AjaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="usercontrol/ucpager.ascx" %>

<ASP:CONTENT ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript" src="../cssscript/jquery.ui.core.js"></script>
    <script type="text/javascript" src="../cssscript/jquery.ui.widget.js"></script>
    <script type="text/javascript" src="../cssscript/jquery.ui.datepicker.js"></script>
    <link type="text/css" rel="stylesheet" href="../cssscript/jquery.ui.css" />

	<script type="text/javascript">
	function createDateform() {
		$( "#<%=txtDatefr.ClientID %>" ).datepicker({
			showOn: "button",
			buttonImage: "../images/calendar.gif",
			dateFormat: "dd/mm/yy",
			showOtherMonths: true,
			selectOtherMonths: true,
			changeMonth: true,
			changeYear: true,
			buttonImageOnly: true
		});
		$( "#<%=txtDateto.ClientID %>" ).datepicker({
			showOn: "button",
			buttonImage: "../images/calendar.gif",
			dateFormat: "dd/mm/yy",
			showOtherMonths: true,
			selectOtherMonths: true,
			changeMonth: true,
			changeYear: true,
			buttonImageOnly: true
		});
	}
	$(function() {
	    createDateform();
	});

    function EndRequestHandler(sender, args) {
        createDateform();
    }
	
	</script>
	
    <ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
        <CONTENTTEMPLATE>
        
        <ASP:LABEL ID="lblError" runat="server" CssClass="dformnoticeb"></ASP:LABEL>
        <ASP:PANEL id="pnlList" runat="server" width="100%">
            <div class="dformoutcommand">
                <div class="colfilter">
                    <ASP:PANEL ID="panelSearch" runat="server" DefaultButton="cmdSearch" CssClass="tformin tformincom">
                        <div class="rowhor">
                            <div class="title">Danh mục:</div>
                            <div class="boxctrlc"><ASP:DROPDOWNLIST ID="ddlCid" runat="server"></ASP:DROPDOWNLIST></div>
                        </div>
                        <div class="rowhor">
                            <div class="title">Ngày cập nhật từ:</div>
                            <div class="boxctrls"><ASP:TEXTBOX ID="txtDatefr" runat="server" Width="100px"></ASP:TEXTBOX></div>
                            <div class="boxctrl50" style="margin-left:15px;">đến:</div>
                            <div class="boxctrls"><ASP:TEXTBOX ID="txtDateto" runat="server" Width="100px"></ASP:TEXTBOX></div>
                        </div>
                        <div class="rowhor">
                            <div class="title">Báo cáo theo:</div>
                            <div class="boxctrll"><ASP:RADIOBUTTONLIST ID="radReporttype" runat="server" RepeatDirection="Horizontal">
                                <ASP:LISTITEM Value="allowcomment" Selected="True">Bình chọn nhiều nhất</ASP:LISTITEM>
                                <ASP:LISTITEM Value="viewcounter">Lượt xem nhiều nhất</ASP:LISTITEM>
                            </ASP:RADIOBUTTONLIST></div>
                        </div>
                        <div class="rowhor">
                            <div class="title">Liệt kê:</div>
                            <div class="boxctrls"><ASP:TEXTBOX ID="txtPagesize" runat="server" Width="100px" Text="20"></ASP:TEXTBOX></div>
                            <div class="boxctrls">tin đầu tiên</div>
                        </div>
                        <div class="rowhor">
                            <div class="title"></div>
                            <div class="boxctrll"><ASP:BUTTON ID="cmdSearch" runat="server" OnClientClick="javascript:return doSubmit();" OnClick="cmdSearch_Click" CssClass="button" Text="Xem thống kê" /> <ASP:BUTTON ID="cmdExport" runat="server" OnClientClick="javascript:return doSubmit();" OnClick="cmdExport_Click" CssClass="button" Text="Xuất tập tin Excel" /></div>
                        </div>
                        <AJAXTOOLKIT:MASKEDEDITEXTENDER ID="MaskedEditExtender1" runat="server" TargetControlID="txtDatefr" Mask="99/99/9999" MaskType="Date" />
                        <AJAXTOOLKIT:MASKEDEDITEXTENDER ID="MaskedEditExtender2" runat="server" TargetControlID="txtDateto" Mask="99/99/9999" MaskType="Date" />
                        <AJAXTOOLKIT:FILTEREDTEXTBOXEXTENDER ID="FilteredTextBoxExtender1" runat="server" TargetControlID="txtPagesize"
                            FilterType="Custom" FilterMode="ValidChars" ValidChars="1234567890" />
                        <br />&nbsp;
                    </ASP:PANEL>
                </div>
            </div>
            
            <div class="dformoutcommand">
                <div class="colfilter">
                    <ASP:HYPERLINK ID="lnkExport" runat="server" Visible="false"></ASP:HYPERLINK>
                </div>  
            </div>
            <div class="dformoutlist">
            <ASP:GRIDVIEW ID="grdView" runat="server" DataKeyNames="Id" CssClass="lgridview" 
                AutoGenerateColumns="false" AllowSorting="true" AllowPaging="false" 
                OnRowDataBound="grdView_RowDataBound" OnSorting="grdView_Sorting" >
                <COLUMNS>
                    <ASP:TEMPLATEFIELD HeaderText="#" HeaderStyle-CssClass="colnum" ItemStyle-CssClass="colnum">
                        <ITEMTEMPLATE>
                            <%# Eval("Rownumber")%>
                            <ASP:HIDDENFIELD ID="txtId" runat="server" Value='<%#Eval("Id") %>' />
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:TEMPLATEFIELD headertext="Tên/ Tiêu đề">
                        <ITEMTEMPLATE>
                            <div><a><%#Eval("Name") %></a> <%#Eval("Iconex").ToString() == "" ? "" : "<img src='../../" + Eval("Iconex") + "' />" %></div>
                            <div><%#Eval("Introduce") %></div>
                        </ITEMTEMPLATE>
                    </ASP:TEMPLATEFIELD>
                    <ASP:BOUNDFIELD datafield="Allowcomment" headertext="Bình chọn" itemstyle-cssclass="colstatus" HtmlEncode="false"></ASP:BOUNDFIELD>
                    <ASP:BOUNDFIELD datafield="Viewcounter" headertext="Lượt xem" itemstyle-cssclass="colstatus" HtmlEncode="false"></ASP:BOUNDFIELD>
                    <ASP:TEMPLATEFIELD headertext="Cập nhật" itemstyle-cssclass="colupdate">
                        <ITEMTEMPLATE>
                            <%#Eval("eTimeupdate") %></a>
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

        </CONTENTTEMPLATE>
    </ASP:UPDATEPANEL>
    
</ASP:CONTENT>
