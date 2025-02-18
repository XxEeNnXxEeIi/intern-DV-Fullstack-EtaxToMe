import { Outlet } from "react-router-dom"
import { Navbar } from "../components"

const HomeLayout = () => {
  return (
    <div className="max-w-full mx-auto">
    <Navbar /> {/* This will be visible on all child routes */}
    <Outlet /> {/* Renders the matching child route*/}
    </div>
  )
}

export default HomeLayout
