import { useAuth } from "../../context/AuthContext";

export default function DashboardPage() {

    const {
        user,
        logout,
    } = useAuth();


    return (
        <div
            style={{
                minHeight: "100vh",
                padding: "40px",
                background: "#070b14",
                color: "#ffffff",
            }}
        >

            <h1>
                DeployNexus Dashboard
            </h1>


            <p>
                Welcome, {user?.username}
            </p>


            <p>
                {user?.email}
            </p>


            <button
                onClick={logout}
                style={{
                    marginTop: "20px",
                    padding: "10px 18px",
                    cursor: "pointer",
                }}
            >
                Logout
            </button>

        </div>
    );
}