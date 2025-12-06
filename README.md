🛍️ Ecommerce-Api
Well-Organized Monolithic Multi-Vendor E-commerce RESTful API
The Ecommerce-Api is a robust, well-organized Monolithic RESTful API developed using C# and .NET Core. It utilizes a 3-Tier Architecture pattern internally to ensure clean separation of concerns, making the application manageable and maintainable. It provides all the core functionalities required for managing products, inventory, shopping carts, orders, vendors, and secure payments for a multi-vendor platform.
🌟 Key Features
Robust Monolithic Architecture: Structured as a single deployment unit, internally organized using a 3-Tier Architecture pattern for strict separation of business, data, and presentation logic.

Core E-commerce Functionality: Implements essential features for products, categories, vendors, shopping carts, orders, and promo codes.

Security & Authorization:

JWT-based Authentication for secure user access.

Role-based Permission Management to implement granular access control over endpoints.

Automated Vendor Accounting: Developed an automated SQL Server Job to calculate vendor profits and platform commissions based on revenue tiers.

Secure Payment Integration (PayPal): Integrated the PayPal payment gateway with secure Webhook Verification.

External System Integration: Integrated with the Store Management System API for critical functions like inventory reservation and order processing.

Performance Optimization: Utilizes Trie Search for fast product searching and includes optimized SQL queries and business logic.

Unit Testing: Ensures system reliability and stability using xUnit for unit testing core components.
Category,Technologies
Language & Framework,"C#, .NET Core"
Database,SQL Server (SSMS)
Data Access,ADO.NET
API Architecture,REST API (Monolithic)
Testing Framework,xUnit
Key Integrations,"JWT, PayPal API, CORS Configuration"