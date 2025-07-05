# Sprint 2

```mermaid
classDiagram
    %% Models (Entities) - Blue
    class User {
        🟦 MODEL
        -int userId
        -string email
        -string passwordHash
        -string firstName
        -string lastName
        -DateTime createdDate
        -bool isActive
        +authenticate(password: string): bool
        +updateProfile(data: UserData): bool
        +resetPassword(): string
    }

    class Role {
        🟦 MODEL
        -int roleId
        -string roleName
        -string description
        -List~Permission~ permissions
        +hasPermission(permission: string): bool
        +addPermission(permission: Permission): void
        +removePermission(permission: Permission): void
    }

    class Permission {
        🟦 MODEL
        -int permissionId
        -string permissionName
        -string description
        -string resource
        -string action
        +validateAccess(resource: string, action: string): bool
    }

    class UserRole {
        🟦 MODEL
        -int userRoleId
        -int userId
        -int roleId
        -int? projectId
        -DateTime assignedDate
        -bool isActive
        +assignRole(userId: int, roleId: int): void
        +revokeRole(): void
    }

    class Project {
        🟦 MODEL
        -int projectId
        -string title
        -string description
        -int clientId
        -ProjectStatus status
        -DateTime startDate
        -DateTime? estimatedEndDate
        -DateTime? actualEndDate
        -decimal estimatedCost
        -decimal actualCost
        +updateStatus(status: ProjectStatus): void
        +assignIntern(userId: int): void
        +calculateProgress(): decimal
    }

    class Client {
        🟦 MODEL
        -int clientId
        -string companyName
        -string contactName
        -string email
        -string phone
        -string address
        -DateTime registrationDate
        +submitProposal(proposal: Proposal): int
        +viewProjectProgress(projectId: int): ProjectProgress
    }

    class ProjectAssignment {
        🟦 MODEL
        -int assignmentId
        -int projectId
        -int userId
        -DateTime assignedDate
        -bool isLeadIntern
        -bool isActive
        +promoteToLead(): void
        +removeFromProject(): void
    }

    %% Controllers - Yellow
    class UserController {
        🟨 CONTROLLER
        -UserService userService
        -AuthenticationService authService
        +login(credentials: LoginData): AuthResult
        +register(userData: UserData): User
        +updateProfile(userId: int, data: UserData): bool
        +manageRoles(userId: int, roles: List~Role~): bool
    }

    class ProjectController {
        🟨 CONTROLLER
        -ProjectService projectService
        -UserService userService
        +createProject(projectData: ProjectData): Project
        +assignInterns(projectId: int, userIds: List~int~): bool
        +updateProjectStatus(projectId: int, status: ProjectStatus): bool
        +getProjectDetails(projectId: int): ProjectDetails
    }

    %% Views - Green
    class LoginView {
        🟩 VIEW
        -string email
        -string password
        +displayLoginForm(): void
        +validateInput(): bool
        +showErrorMessage(message: string): void
    }

    class UserManagementView {
        🟩 VIEW
        -List~User~ users
        -List~Role~ availableRoles
        +displayUserList(): void
        +showRoleAssignmentForm(userId: int): void
        +displayPermissionMatrix(): void
    }

    class ProjectDashboardView {
        🟩 VIEW
        -List~Project~ projects
        -ProjectFilters filters
        +displayProjectList(): void
        +showProjectDetails(projectId: int): void
        +renderGanttChart(tasks: List~Task~): void
    }

    %% Relationships
    User ||--o{ UserRole : has
    Role ||--o{ UserRole : assigned_to
    Role ||--o{ Permission : contains
    User ||--o{ ProjectAssignment : assigned_to
    Project ||--o{ ProjectAssignment : has
    Client ||--o{ Project : owns
    
    UserController --> User : manages
    UserController --> LoginView : updates
    ProjectController --> Project : manages
    ProjectController --> ProjectDashboardView : updates
```
