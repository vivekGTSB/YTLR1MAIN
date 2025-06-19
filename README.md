# YTL Fleet Management System

A modernized and secure fleet management web application built with ASP.NET Web Forms and VB.NET.

## 🚀 Features

- **Secure Authentication**: Modern login system with CSRF protection, rate limiting, and secure session management
- **Role-Based Access Control**: Different access levels for Admin, SuperUser, Operator, and User roles
- **Fleet Management**: Comprehensive vehicle tracking and management capabilities
- **Fuel Management**: Track fuel consumption, costs, and efficiency metrics
- **Reporting**: Advanced reporting capabilities with Excel export functionality
- **Security**: Enhanced security with input validation, XSS protection, and SQL injection prevention

## 🔒 Security Features

- **Input Validation**: All user inputs are validated and sanitized
- **SQL Injection Protection**: Parameterized queries and input sanitization
- **XSS Prevention**: HTML encoding and content security policies
- **CSRF Protection**: Token-based CSRF protection on all forms
- **Rate Limiting**: Protection against brute force attacks
- **Secure Headers**: Comprehensive security headers implementation
- **Session Security**: Secure session management with proper timeouts
- **Audit Logging**: Comprehensive security event logging

## 🛠️ Technology Stack

- **Backend**: ASP.NET Web Forms 4.5.1, VB.NET
- **Database**: SQL Server with encrypted connections
- **Frontend**: Modern responsive design with CSS3 and JavaScript
- **Security**: Forms Authentication with enhanced security measures
- **Deployment**: IIS with security hardening

## 📋 Prerequisites

- .NET Framework 4.5.1 or higher
- SQL Server 2012 or higher
- IIS 7.0 or higher
- Visual Studio 2015 or higher (for development)

## 🚀 Getting Started

1. **Clone the repository**
   ```bash
   git clone [repository-url]
   cd YTLWebApplication
   ```

2. **Database Setup**
   - Create a SQL Server database
   - Update connection strings in `web.config`
   - Run database migration scripts

3. **Configuration**
   - Update `web.config` with your database connection strings
   - Configure SMTP settings for email notifications
   - Set up SSL certificates for HTTPS

4. **Security Configuration**
   - Generate new machine keys for encryption
   - Configure authentication settings
   - Set up proper file permissions

5. **Deploy to IIS**
   - Create an application pool
   - Deploy the application
   - Configure security headers

## 🔧 Configuration

### Database Connection
Update the connection string in `web.config`:
```xml
<add key="sqlserverconnection" value="Data Source=SERVER;Database=DATABASE;Integrated Security=SSPI;Encrypt=True;" />
```

### Security Settings
The application includes comprehensive security settings in `web.config`. Review and adjust based on your environment.

## 📊 Key Modules

- **User Management**: User registration, authentication, and role management
- **Vehicle Management**: Vehicle registration, tracking, and maintenance
- **Fuel Management**: Fuel consumption tracking and cost analysis
- **Reporting**: Comprehensive reporting with data visualization
- **Geofencing**: Location-based alerts and monitoring
- **Maintenance**: Service scheduling and maintenance tracking

## 🔐 Security Best Practices

- All passwords are hashed using SHA-256
- Session tokens are regenerated on login
- CSRF tokens protect all forms
- Input validation prevents injection attacks
- Rate limiting prevents brute force attacks
- Comprehensive audit logging tracks all security events

## 📝 API Documentation

The application includes RESTful endpoints for:
- Vehicle data retrieval
- Fuel consumption reporting
- User management
- Real-time tracking data

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📄 License

This project is proprietary software owned by YTL Corporation.

## 🆘 Support

For technical support or questions, please contact the development team.

## 🔄 Version History

- **v2.0.0**: Major security overhaul and modernization
- **v1.x.x**: Legacy versions (deprecated)

---

**Note**: This application has been modernized with enhanced security features. All legacy security vulnerabilities have been addressed in version 2.0.0.