<%@ Page Language="VB" AutoEventWireup="false" CodeBehind="Dashboard.aspx.vb" Inherits="YTLWebApplication.Dashboard" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>YTL Fleet Management - Dashboard</title>
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
        .user-info {
            display: flex;
            align-items: center;
            gap: 15px;
        }
        .logout-btn {
            background-color: #d32f2f;
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 4px;
            cursor: pointer;
            text-decoration: none;
        }
        .logout-btn:hover {
            background-color: #b71c1c;
        }
        .container {
            max-width: 1200px;
            margin: 20px auto;
            padding: 0 20px;
        }
        .dashboard-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin-top: 20px;
        }
        .dashboard-card {
            background: white;
            border-radius: 8px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        .dashboard-card h3 {
            margin-top: 0;
            color: #333;
            border-bottom: 2px solid #465ae8;
            padding-bottom: 10px;
        }
        .menu-list {
            list-style: none;
            padding: 0;
            margin: 0;
        }
        .menu-list li {
            margin-bottom: 10px;
        }
        .menu-list a {
            display: block;
            padding: 10px 15px;
            background-color: #f8f9fa;
            color: #333;
            text-decoration: none;
            border-radius: 4px;
            transition: background-color 0.3s;
        }
        .menu-list a:hover {
            background-color: #465ae8;
            color: white;
        }
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
            gap: 15px;
            margin-top: 15px;
        }
        .stat-item {
            text-align: center;
            padding: 15px;
            background-color: #f8f9fa;
            border-radius: 4px;
        }
        .stat-number {
            font-size: 24px;
            font-weight: bold;
            color: #465ae8;
        }
        .stat-label {
            font-size: 12px;
            color: #666;
            margin-top: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <h1>YTL Fleet Management System</h1>
            <div class="user-info">
                <span>Welcome, <asp:Label ID="lblUsername" runat="server" /></span>
                <span>|</span>
                <span>Role: <asp:Label ID="lblRole" runat="server" /></span>
                <asp:LinkButton ID="btnLogout" runat="server" CssClass="logout-btn" OnClick="btnLogout_Click">Logout</asp:LinkButton>
            </div>
        </div>
        
        <div class="container">
            <div class="dashboard-grid">
                <div class="dashboard-card">
                    <h3>Reports</h3>
                    <ul class="menu-list">
                        <li><a href="ECRLReport.aspx">ECRL Report</a></li>
                        <li><a href="FleetMoniterReport.aspx">Fleet Monitor Report</a></li>
                        <li><a href="FuelAnalysisReportChart.aspx">Fuel Analysis Report</a></li>
                    </ul>
                </div>
                
                <div class="dashboard-card">
                    <h3>Management</h3>
                    <ul class="menu-list">
                        <li><a href="EPodManagementNew.aspx">e-POD Management</a></li>
                        <li><a href="FuelManagement.aspx">Fuel Management</a></li>
                    </ul>
                </div>
                
                <div class="dashboard-card">
                    <h3>System Statistics</h3>
                    <div class="stats-grid">
                        <div class="stat-item">
                            <div class="stat-number" id="totalVehicles" runat="server">0</div>
                            <div class="stat-label">Total Vehicles</div>
                        </div>
                        <div class="stat-item">
                            <div class="stat-number" id="activeUsers" runat="server">0</div>
                            <div class="stat-label">Active Users</div>
                        </div>
                        <div class="stat-item">
                            <div class="stat-number" id="todayReports" runat="server">0</div>
                            <div class="stat-label">Today's Reports</div>
                        </div>
                    </div>
                </div>
                
                <div class="dashboard-card">
                    <h3>Quick Actions</h3>
                    <ul class="menu-list">
                        <li><a href="ExcelReport.aspx">Download Reports</a></li>
                        <li><a href="FuelChart3.aspx">View Charts</a></li>
                    </ul>
                </div>
            </div>
        </div>
    </form>
    
    <script type="text/javascript">
        // Auto-refresh statistics every 5 minutes
        setInterval(function() {
            window.location.reload();
        }, 300000);
    </script>
</body>
</html>