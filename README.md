
# Lockshot Application

Lockshot is a comprehensive application designed for shooting sports enthusiasts. It provides features for tracking training progress, engaging with a community of like-minded individuals, and leveraging AI-driven insights to improve your skills.

---

## Features

### Web and Mobile Platforms
The application is available as a **web app** and a **mobile app** (for Android) built using:
- **Web Frontend**: React.js
- **Mobile Frontend**: React Native

---

### Core Functionality
1. **Shooting Training Tracker**  
   - Log your shooting hits.
   - Analyze and track hit statistics.

2. **Community Interaction**  
   - Chat with other athletes via a real-time chat system powered by SignalR.
   - Discuss strategies, techniques, and share experiences.

3. **Educational Resources**  
   - Access videos to learn about shooting sports techniques.
   - Stay updated with the latest news in the shooting sports world.

4. **AI Assistant**  
   - Get personalized training tips and advice powered by an AI assistant.

---

### Backend Architecture
The backend is implemented using **.NET Core** with the following components:
- **Messaging System**: SignalR for real-time communication.
- **Data Storage**: PostgreSQL as the database.
- **Web API**: RESTful services for all features.
- **AI Integration**: Leverages modern AI frameworks to power the assistant.
- **Redis**: For caching.

#### Backend Microservices
The application employs a microservices architecture, including:
- `Lockshot.User.API`  
- `Lockshot.Channels.API`  
- `Lockshot.Client.Web.API`  
- `Lockshot.Bot.API`

---

## Technologies
### Frontend
- **React.js** and **React Native** (with Redux and React Router).

### Backend
- **.NET Core**  
- **Entity Framework Core**  
- **SignalR** for real-time features.  
- **Redis** for caching.  
- **PostgreSQL** for data storage.  
- **Docker** for containerized deployments.

---

## Setup and Deployment
1. Clone the repository:
   ```bash
   git clone https://github.com/KovganAV/Lockshot.User.API
   cd lockshot
   ```

2. Set up the backend:
   - Navigate to the backend service directories and build them using `dotnet build`.
   - Run the services with `docker-compose` for seamless deployment.

3. Set up the frontend:
   - Navigate to the `frontend` and `mobile` directories.
   - Install dependencies:
     ```bash
     npm install
     ```
   - Start the development server:
     ```bash
     npm start
     ```

4. Configure environment variables for APIs, database connections, and AI integrations.

---

## Future Enhancements
- Add more AI-driven insights.
- Expand community features with group forums.
- Optimize the mobile app for cross-platform support (iOS).

---

## License
This project is licensed under the MIT License.

---

## Contributions
Contributions are welcome! Please open a pull request or report issues for discussion.

---

**Enjoy using Lockshot and take your shooting skills to the next level!** 🎯
