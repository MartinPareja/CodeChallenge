# Stage 1: Build the Angular application
FROM node:20-alpine AS builder

# Set the working directory
WORKDIR /app

# Copy package.json and package-lock.json to the container
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code
COPY . .

# Build the application for production
RUN npm run build

# Stage 2: Serve the application with Nginx
FROM nginx:stable-alpine

# Copy the built Angular app from the builder stage
COPY --from=builder /app/dist/rental-app/browser /usr/share/nginx/html

# Copy a custom Nginx configuration file
COPY ./nginx.conf /etc/nginx/nginx.conf

# Expose port 80
EXPOSE 80