<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.AVLS.FuelFormulaManagement" Codebehind="FuelFormulaManagement.aspx.vb" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Fuel Formula Management - YTL Fleet Management</title>
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
        .form-group {
            margin-bottom: 15px;
        }
        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: bold;
            color: #555;
        }
        .form-group select {
            width: 100%;
            padding: 8px;
            border: 1px solid #ddd;
            border-radius: 4px;
            font-size: 14px;
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
    </style>
    <script type="text/javascript">
        function deleteconfirmation() {
            return confirm("Are you sure you want to delete the selected items?");
        }
        
        function validateForm() {
            var userSelect = document.getElementById('<%= ddlusers.ClientID %>');
            if (userSelect.selectedIndex === 0) {
                alert("Please select a user.");
                return false;
            }
            return true;
        }
    </script>
</head>
<body>
    <form id="vehicleform" runat="server">
        <div class="header">
            <h1>Fuel Formula Management</h1>
            <a href="~/Dashboard.aspx" class="btn btn-primary" runat="server">Back to Dashboard</a>
        </div>
        
        <div class="container">
            <div class="form-section">
                <h3>Select User</h3>
                <div class="form-group">
                    <label for="ddlusers">User Name:</label>
                    <asp:DropDownList ID="ddlusers" runat="server" AutoPostBack="True" CssClass="form-control">
                        <asp:ListItem Value="">--Select User Name--</asp:ListItem>
                    </asp:DropDownList>
                </div>
                
                <asp:Button ID="ImageButton1" runat="server" Text="Load Vehicles" CssClass="btn btn-primary" OnClientClick="return validateForm();" />
            </div>
            
            <div class="grid-container">
                <h3>Fuel Formula Configuration</h3>
                <div style="margin-bottom: 15px;">
                    <asp:Button ID="ImageButton2" runat="server" Text="Delete Selected" CssClass="btn btn-danger" OnClientClick="return deleteconfirmation();" />
                </div>
                
                <asp:GridView ID="vehiclesgrid" runat="server" AutoGenerateColumns="False" 
                             CssClass="grid-table" EnableViewState="False"
                             EmptyDataText="No fuel formula data available.">
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server" CssClass="checkbox" />
                                <asp:HiddenField ID="hdnValue" runat="server" Value='<%# Eval("PlateNo") & ";" & Eval("Tank") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="S No" HeaderText="S No" />
                        <asp:BoundField DataField="Plate No" HeaderText="Plate No" HtmlEncode="true" />
                        <asp:BoundField DataField="Level ID" HeaderText="Level ID" HtmlEncode="true" />
                        <asp:BoundField DataField="Formula" HeaderText="Formula" HtmlEncode="true" />
                        <asp:BoundField DataField="Offset Value" HeaderText="Offset Value" HtmlEncode="true" />
                        <asp:BoundField DataField="Type" HeaderText="Type" HtmlEncode="true" />
                        <asp:BoundField DataField="Tank" HeaderText="Tank" HtmlEncode="true" />
                        <asp:BoundField DataField="Remarks" HeaderText="Remarks" HtmlEncode="true" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
        
        <asp:HiddenField ID="hdnCSRFToken" runat="server" />
    </form>
</body>
</html>