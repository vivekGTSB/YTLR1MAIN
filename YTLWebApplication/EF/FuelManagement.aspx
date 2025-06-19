<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.FuelManagement" Codebehind="FuelManagement.aspx.vb" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fuel Management - YTL Fleet Management</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="robots" content="noindex, nofollow" />
    <style type="text/css">
        body {
            font-family: Arial, sans-serif;
            background-color: #f5f5f5;
            margin: 0;
            padding: 0;
        }
        .header {
            background-color: #465ae8;
            color: white;
            padding: 15px 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        .header h1 {
            margin: 0;
            font-size: 24px;
        }
        .container {
            max-width: 1200px;
            margin: 20px auto;
            padding: 0 20px;
        }
        .form-section {
            background: white;
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        .form-row {
            display: flex;
            gap: 15px;
            margin-bottom: 15px;
            flex-wrap: wrap;
        }
        .form-group {
            flex: 1;
            min-width: 200px;
        }
        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: bold;
            color: #555;
        }
        .form-group input,
        .form-group select {
            width: 100%;
            padding: 8px;
            border: 1px solid #ddd;
            border-radius: 4px;
            font-size: 14px;
            box-sizing: border-box;
        }
        .btn {
            padding: 10px 20px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            margin-right: 10px;
        }
        .btn-primary {
            background-color: #465ae8;
            color: white;
        }
        .btn-success {
            background-color: #28a745;
            color: white;
        }
        .btn-danger {
            background-color: #d32f2f;
            color: white;
        }
        .btn:hover {
            opacity: 0.9;
        }
        .grid-container {
            background: white;
            border-radius: 8px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        .grid-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 15px;
        }
        .grid-table th,
        .grid-table td {
            padding: 10px;
            text-align: left;
            border-bottom: 1px solid #ddd;
        }
        .grid-table th {
            background-color: #465ae8;
            color: white;
            font-weight: bold;
        }
        .grid-table tr:hover {
            background-color: #f5f5f5;
        }
        .checkbox {
            margin-right: 5px;
        }
        .pagination {
            margin-top: 15px;
            text-align: center;
        }
        .pagination a {
            display: inline-block;
            padding: 8px 12px;
            margin: 0 2px;
            text-decoration: none;
            border: 1px solid #ddd;
            color: #465ae8;
        }
        .pagination a:hover {
            background-color: #465ae8;
            color: white;
        }
    </style>
    <script type="text/javascript">
        function mysubmit() {
            var username = document.getElementById('<%= ddlusername.ClientID %>');
            var plateno = document.getElementById('<%= ddlpleate.ClientID %>');
            
            if (username.selectedIndex === 0) {
                alert("Please select user name");
                return false;
            }
            
            if (plateno.selectedIndex === 0) {
                alert("Please select plate number");
                return false;
            }
            
            var beginDate = document.getElementById('<%= txtBeginDate.ClientID %>').value;
            var endDate = document.getElementById('<%= txtEndDate.ClientID %>').value;
            
            if (!beginDate || !endDate) {
                alert("Please select both begin and end dates");
                return false;
            }
            
            var beginDateTime = new Date(beginDate + " " + document.getElementById('<%= ddlbh.ClientID %>').value + ":" + document.getElementById('<%= ddlbm.ClientID %>').value);
            var endDateTime = new Date(endDate + " " + document.getElementById('<%= ddleh.ClientID %>').value + ":" + document.getElementById('<%= ddlem.ClientID %>').value);
            
            var diff = (endDateTime - beginDateTime) / (1000 * 60 * 60 * 24);
            var days = parseInt(diff) + 1;
            
            if (days > 7) {
                return confirm("You selected " + days + " days of data. This may take more time to execute.\nAre you sure you want to proceed?");
            }
            
            return true;
        }
        
        function deleteconfirmation() {
            return confirm("Are you sure you want to delete the selected fuel records?");
        }
        
        function validateDownload() {
            var ec = '<%= ec %>';
            if (ec === "true") {
                return true;
            } else {
                alert("Please click Submit button to obtain latest result.");
                return false;
            }
        }
    </script>
</head>
<body>
    <form id="fuelform" runat="server">
        <div class="header">
            <h1>Fuel Management</h1>
            <a href="~/Dashboard.aspx" class="btn btn-primary" runat="server">Back to Dashboard</a>
        </div>
        
        <div class="container">
            <div class="form-section">
                <h3>Fuel Data Filter</h3>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="txtBeginDate">Begin Date:</label>
                        <asp:TextBox ID="txtBeginDate" runat="server" TextMode="Date" CssClass="form-control" />
                    </div>
                    <div class="form-group">
                        <label for="ddlbh">Begin Hour:</label>
                        <asp:DropDownList ID="ddlbh" runat="server" CssClass="form-control">
                            <asp:ListItem Value="00">00</asp:ListItem>
                            <asp:ListItem Value="01">01</asp:ListItem>
                            <asp:ListItem Value="02">02</asp:ListItem>
                            <asp:ListItem Value="03">03</asp:ListItem>
                            <asp:ListItem Value="04">04</asp:ListItem>
                            <asp:ListItem Value="05">05</asp:ListItem>
                            <asp:ListItem Value="06">06</asp:ListItem>
                            <asp:ListItem Value="07" Selected="True">07</asp:ListItem>
                            <asp:ListItem Value="08">08</asp:ListItem>
                            <asp:ListItem Value="09">09</asp:ListItem>
                            <asp:ListItem Value="10">10</asp:ListItem>
                            <asp:ListItem Value="11">11</asp:ListItem>
                            <asp:ListItem Value="12">12</asp:ListItem>
                            <asp:ListItem Value="13">13</asp:ListItem>
                            <asp:ListItem Value="14">14</asp:ListItem>
                            <asp:ListItem Value="15">15</asp:ListItem>
                            <asp:ListItem Value="16">16</asp:ListItem>
                            <asp:ListItem Value="17">17</asp:ListItem>
                            <asp:ListItem Value="18">18</asp:ListItem>
                            <asp:ListItem Value="19">19</asp:ListItem>
                            <asp:ListItem Value="20">20</asp:ListItem>
                            <asp:ListItem Value="21">21</asp:ListItem>
                            <asp:ListItem Value="22">22</asp:ListItem>
                            <asp:ListItem Value="23">23</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label for="ddlbm">Begin Minute:</label>
                        <asp:DropDownList ID="ddlbm" runat="server" CssClass="form-control">
                            <asp:ListItem Value="00" Selected="True">00</asp:ListItem>
                            <asp:ListItem Value="15">15</asp:ListItem>
                            <asp:ListItem Value="30">30</asp:ListItem>
                            <asp:ListItem Value="45">45</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="txtEndDate">End Date:</label>
                        <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="form-control" />
                    </div>
                    <div class="form-group">
                        <label for="ddleh">End Hour:</label>
                        <asp:DropDownList ID="ddleh" runat="server" CssClass="form-control">
                            <asp:ListItem Value="00">00</asp:ListItem>
                            <asp:ListItem Value="01">01</asp:ListItem>
                            <asp:ListItem Value="02">02</asp:ListItem>
                            <asp:ListItem Value="03">03</asp:ListItem>
                            <asp:ListItem Value="04">04</asp:ListItem>
                            <asp:ListItem Value="05">05</asp:ListItem>
                            <asp:ListItem Value="06">06</asp:ListItem>
                            <asp:ListItem Value="07">07</asp:ListItem>
                            <asp:ListItem Value="08">08</asp:ListItem>
                            <asp:ListItem Value="09">09</asp:ListItem>
                            <asp:ListItem Value="10">10</asp:ListItem>
                            <asp:ListItem Value="11">11</asp:ListItem>
                            <asp:ListItem Value="12">12</asp:ListItem>
                            <asp:ListItem Value="13">13</asp:ListItem>
                            <asp:ListItem Value="14">14</asp:ListItem>
                            <asp:ListItem Value="15">15</asp:ListItem>
                            <asp:ListItem Value="16">16</asp:ListItem>
                            <asp:ListItem Value="17">17</asp:ListItem>
                            <asp:ListItem Value="18">18</asp:ListItem>
                            <asp:ListItem Value="19">19</asp:ListItem>
                            <asp:ListItem Value="20">20</asp:ListItem>
                            <asp:ListItem Value="21">21</asp:ListItem>
                            <asp:ListItem Value="22">22</asp:ListItem>
                            <asp:ListItem Value="23" Selected="True">23</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label for="ddlem">End Minute:</label>
                        <asp:DropDownList ID="ddlem" runat="server" CssClass="form-control">
                            <asp:ListItem Value="00">00</asp:ListItem>
                            <asp:ListItem Value="15">15</asp:ListItem>
                            <asp:ListItem Value="30">30</asp:ListItem>
                            <asp:ListItem Value="59" Selected="True">59</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="ddlusername">User Name:</label>
                        <asp:DropDownList ID="ddlusername" runat="server" AutoPostBack="True" CssClass="form-control">
                            <asp:ListItem Value="">--Select User Name--</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label for="ddlpleate">Plate Number:</label>
                        <asp:DropDownList ID="ddlpleate" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">--Select Plate No--</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label for="noofrecords">Records per Page:</label>
                        <asp:DropDownList ID="noofrecords" runat="server" CssClass="form-control">
                            <asp:ListItem Value="10">10</asp:ListItem>
                            <asp:ListItem Value="25" Selected="True">25</asp:ListItem>
                            <asp:ListItem Value="50">50</asp:ListItem>
                            <asp:ListItem Value="100">100</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                
                <div style="margin-top: 20px;">
                    <asp:Button ID="ImageButton1" runat="server" Text="Submit" CssClass="btn btn-primary" OnClientClick="return mysubmit();" />
                    <asp:Button ID="btnDownload" runat="server" Text="Download Excel" CssClass="btn btn-success" OnClientClick="return validateDownload();" />
                    <asp:Button ID="delete1" runat="server" Text="Delete Selected" CssClass="btn btn-danger" OnClientClick="return deleteconfirmation();" />
                    <a href="<%= addfuelpage %>" class="btn btn-success">Add New Fuel Record</a>
                </div>
            </div>
            
            <div class="grid-container">
                <h3>Fuel Records</h3>
                
                <asp:GridView ID="fuelgrid" runat="server" AutoGenerateColumns="False" 
                             CssClass="grid-table" EnableViewState="False" AllowPaging="True"
                             EmptyDataText="No fuel records found for the selected criteria.">
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server" CssClass="checkbox" />
                                <asp:HiddenField ID="hdnFuelId" runat="server" Value='<%# Eval("Fuel Id") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="S No" HeaderText="S No" />
                        <asp:BoundField DataField="Plate No" HeaderText="Plate No" HtmlEncode="true" />
                        <asp:BoundField DataField="Date Time" HeaderText="Date Time" HtmlEncode="true" />
                        <asp:BoundField DataField="Fuel Id" HeaderText="Fuel Id" HtmlEncode="true" />
                        <asp:BoundField DataField="Fuel Station Code" HeaderText="Station Code" HtmlEncode="true" />
                        <asp:BoundField DataField="Fuel Type" HeaderText="Fuel Type" HtmlEncode="true" />
                        <asp:BoundField DataField="Liters" HeaderText="Liters" HtmlEncode="true" />
                        <asp:BoundField DataField="Cost" HeaderText="Cost (RM)" HtmlEncode="true" />
                    </Columns>
                    <PagerStyle CssClass="pagination" />
                </asp:GridView>
                
                <div style="margin-top: 15px;">
                    <asp:Button ID="delete2" runat="server" Text="Delete Selected" CssClass="btn btn-danger" OnClientClick="return deleteconfirmation();" />
                </div>
            </div>
        </div>
        
        <asp:HiddenField ID="hdnCSRFToken" runat="server" />
    </form>
</body>
</html>