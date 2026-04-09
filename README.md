### **Core Features**
-   **🔐 Zero-Trust Security**: Implemented with ASP.NET Core Identity & JWT Bearer authentication.
-   **📈 Management Dashboards**: Dynamic, role-based interfaces for Admins, Employers, and Students.
-   **🎓 Career-Linked Education**: A many-to-many relationship system linking courses to job qualifications and vice versa.
-   **⚡ Modern SPA Frontend**: Single-page architecture with Mystic Jade themes and glassmorphism.

---

### **Dashboards & Interface**

#### **Admin Dashboard**
Global oversight of system metrics, student enrollment, and job statistics.
![Admin Dashboard](./screenshots/admin_dashboard.png)

#### **Employer Console**
Streamlined hub for posting vacancies, managing internal course catalogs, and linking upskilling paths to roles.
![Employer Dashboard](./screenshots/employer_dashboard.png)

---

### **Setup & Installation**

**1. Backend Initialization**
*   Open `appsettings.json` and verify `DefaultConnection`.
*   Run `Update-Database` in Package Manager Console to apply the **Many-to-Many Linking** migrations.

**2. Frontend Launch**
*   Ensure the backend is running (`http://localhost:5178`).
*   Open `Frontend/index.html` in any modern browser.

---

### **API Architecture**
| Category | Endpoint | Method | Role | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Auth** | `/api/Account/register` | POST | Public | Create accounts (Student/Employer/Admin). |
| **Logic** | `/api/Jobs` | GET | Auth | Fetch jobs with recommended courses. |
| **Logic** | `/api/Courses` | GET | Public | Fetch courses with career insights. |
| **Admin** | `/api/Admin/stats` | GET | Admin | System-wide performance metrics. |

---

### **Technical Implementation**
*   **Entity Framework Core**: Powering the `JobPost <-> JobPostCourse <-> Course` relationship.
*   **JWT Security**: Tokens include role claims for client-side routing and server-side authorization.
*   **Responsive Styling**: CSS-native gradients and fluid layouts for a premium experience.