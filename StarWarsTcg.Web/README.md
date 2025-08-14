# StarWarsTcgWeb

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 19.2.12.

## Folder structure
my-angular-app/
│
├── e2e/                          # Contains end-to-end tests for the application
│   ├── src/
│   └── protractor.conf.js
│
├── node_modules/                # Node.js modules
│
├── src/
│   ├── app/                      # The main application folder where all the Angular components, services, and modules reside
│   │   ├── core/                 # Contains singleton services and shared functionality that can be used throughout the application
│   │   │   ├── services/         # Shared services (e.g., AuthService)
│   │   │   ├── guards/           # Route guards (e.g., AuthGuard)
│   │   │   └── interceptors/     # HTTP interceptors
│   │   │
│   │   ├── shared/               # Contains reusable components, directives, and pipes that can be used across different modules
│   │   │   ├── components/       # Reusable components (e.g., buttons, modals)
│   │   │   ├── directives/       # Custom directives
│   │   │   └── pipes/            # Custom pipes
│   │   │
│   │   ├── features/             # Contains feature-specific modules, each encapsulating related components and services
│   │   │   ├── auth/             # Authentication module, Handles authentication-related components and services
│   │   │   │   ├── login/        # Login component
│   │   │   │   ├── logout/       # Logout component
│   │   │   │   └── auth.module.ts # Auth module
│   │   │   │
│   │   │   ├── admin/            # Administration module, Contains components and services for site administration
│   │   │   │   ├── dashboard/     # Admin dashboard component
│   │   │   │   ├── users/         # User management component
│   │   │   │   └── admin.module.ts # Admin module
│   │   │   │
│   │   │   ├── games/            # Games module, Contains components and services related to playing and watching games
│   │   │   │   ├── game-list/    # List of games component
│   │   │   │   ├── game-detail/   # Game detail component
│   │   │   │   └── games.module.ts # Games module
│   │   │   │
│   │   │   ├── leaderboard/      # component for Welcome view, displays Dark-side and Light-side W/L/T records for top players
│   │   │   │
│   │   │   └──
│   │   │
│   │   ├── layouts/              # Layout components (e.g., header, footer), Contains layout components that define the structure of different pages
│   │   │   ├── main-layout/      # Main layout component
│   │   │   └── admin-layout/     # Admin layout component
│   │   │
│   │   ├── app-routing.module.ts  # Main routing module
│   │   ├── app.component.ts       # Root component
│   │   ├── app.module.ts          # Root module
│   │   └── app.component.html      # Root component template
│   │
│   ├── assets/                    # Static assets (images, fonts, etc.), Contains static assets like images and fonts
│   ├── environments/              # Environment configuration, Contains environment-specific configuration files
│   │   ├── environment.ts         # Development environment
│   │   └── environment.prod.ts    # Production environment
│   │
│   ├── styles/                    # Global styles
│   ├── index.html                 # Main HTML file
│   └── main.ts                    # Main entry point
│
├── angular.json                   # Angular CLI configuration
├── package.json                   # Project metadata and dependencies
├── tsconfig.json                  # TypeScript configuration
└── README.md                      # Project documentation


## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Karma](https://karma-runner.github.io) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
