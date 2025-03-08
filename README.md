# Etax-to-me Project | โปรเจค Etax-to-me

## Overview | เกี่ยวกับโปรเจค
This project was part of my 3-month internship at **Digital Value**. I worked on the **Etax-to-me** project, contributing to both back-end and front-end development.  
โปรเจคนี้เป็นส่วนหนึ่งของการฝึกงาน 3 เดือนที่บริษัท **Digital Value** ซึ่งฉันมีส่วนร่วมในโปรเจค **Etax-to-me** ในการพัฒนาด้าน back-end และ front-end

### My Role | บทบาทของฉัน
- **Back-End Development** (2 months): Worked on document management using **ASP.Net**.  
  - พัฒนา back-end ด้วย **ASP.Net** ในการจัดการเอกสาร
- **Front-End Development** (1 month): Developed the front-end using **React Vite**.  
  - พัฒนา front-end ด้วย **React Vite**

## Key Features | คุณสมบัติหลัก
- 📑 **Document Management**: Efficient handling and organization of e-documents.  
- 🖥️ **Responsive UI**: Fully responsive interface that adapts to various devices.  
- 🚀 **Optimized Performance**: Quick page load times and efficient data management.

## Technologies Used | เทคโนโลยีที่ใช้
- 🖥️ **ASP.Net** (Back-End)  
- ⚛️ **React Vite** (Front-End)  
- 🛠️ **Swagger** (API Documentation)  
- 🐳 **Docker** (Containerization)  
- 📄 **Document Management**  
- 🌐 **RESTful APIs** (Communication between front-end and back-end)

## Swagger Integration | การใช้งาน Swagger
For API documentation, we use **Swagger** to generate and document our RESTful APIs. Swagger helps to easily test endpoints and understand the structure of API responses.  

- 🔑 **Endpoints**: Every API endpoint is clearly documented.  
- 📚 **Interactive Documentation**: Swagger allows testing API methods directly in the browser.

To access the Swagger documentation, navigate to:  
`http://localhost:5000/swagger` (Local server)

## Docker Image Build | การสร้าง Docker Image
Docker is used to containerize the application for a consistent environment across different systems. Follow the steps below to build and run the Docker container.

### Build the Docker Image
1. Clone the repository:  
   `git clone https://github.com/yourusername/Etax-to-me.git`
2. Navigate to the project directory:  
   `cd Etax-to-me`
3. Build the Docker image:  
   `docker build -t etax-to-me .`
   
### Run the Docker Container
After building the image, you can run it using the following command:
```bash
docker run -d -p 8080:80 etax-to-me
