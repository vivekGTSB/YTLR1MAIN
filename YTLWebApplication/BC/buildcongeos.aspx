<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="buildcongeos.aspx.vb" Inherits="YTLWebApplication.buildcongeos" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Geofence Visit Report</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet"/>
    <meta http-equiv="X-Content-Type-Options" content="nosniff">
    <meta http-equiv="X-Frame-Options" content="DENY">
    <meta http-equiv="X-XSS-Protection" content="1; mode=block">
</head>
<body class="bg-light p-4">
    <form runat="server">
        <%=GetCSRFTokenHtml()%>
        <div class="container">
            <h2 class="mb-4">Geofence Visit Report for BuildCon</h2>
            
            <div class="table-responsive">
                <table class="table table-bordered table-striped" id="results">
                    <thead class="table-dark">
                        <tr>
                            <th>DN No</th>
                            <th>Geofence Name</th>
                            <th>Plate No</th>
                            <th>In Timestamp</th>
                            <th>Out Timestamp</th>
                            <th>Duration (min)</th>
                        </tr>
                    </thead>
                    <tbody id="tableBody">
                        <%=SafeOutput(sb)%>
                    </tbody>
                </table>
            </div>
        </div>
    </form>
</body>
</html>