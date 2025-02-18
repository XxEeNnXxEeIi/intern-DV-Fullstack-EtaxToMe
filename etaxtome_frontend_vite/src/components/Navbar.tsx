import React from 'react';
import { NavLink, useLocation } from 'react-router-dom';
import { useApiKey } from '../context/ApiKeyContext';

const NavLinks = () => {
  const location = useLocation();
  const queryParams = location.search; // Store the current query params
  const { publicApiKey } = useApiKey();

  // Define the navigation links
  const links = [
    { id: 1, url: '/document/get', text: 'รายการคำขอ' },
    { id: 2, url: '/document/add', text: 'สร้างคำขอใหม่' },
  ];

  return (
    <>
      {links.map((link) => (
        <li key={link.id}>
          <NavLink
            to={`${link.url}/?publicApiKey=${publicApiKey}`} // Append the current query parameters to the URL
            className={({ isActive }) =>
              `text-white px-3 py-2 rounded transition-colors duration-200 ${isActive ? 'bg-[#0C3764]' : 'hover:bg-[#e78b4c]'}` // Active/inactive styles
            }
          >
            {link.text}
          </NavLink>
        </li>
      ))}
    </>
  );
};

const Navbar = () => {
  const location = useLocation();
  const queryParams = location.search; // Store the current query params
  const { publicApiKey } = useApiKey(); // Destructure corpData, loading, and error from context

  return (
    <nav className="bg-[#FEAB71] p-2">
      <div className="container mx-auto flex items-center justify-between max-w-3xl">
        {/* Navbar Start - Logo */}
        <div className="navbar-start">
        <NavLink to={`/?publicApiKey=${publicApiKey}`} className="text-white text-2xl font-bold">
            ETAX
          </NavLink>
        </div>

        {/* Navbar Center - Links */}
        <div className="navbar-center">
          <ul className="menu menu-horizontal flex space-x-4">
            <NavLinks /> {/* Render the navigation links with query params */}
          </ul>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
