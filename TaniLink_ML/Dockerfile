# Use an official Python runtime as a parent image
FROM python:3.10

# Set the working directory to /app
WORKDIR /app

# Install Git to clone the repository
RUN apt-get update \
    && apt-get install -y curl gnupg \
    && apt-get install -y git

# Install ODBC drivers for SQL Server

RUN curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg \
    && curl https://packages.microsoft.com/config/debian/12/prod.list > /etc/apt/sources.list.d/mssql-release.list \
    && apt-get update \
    && ACCEPT_EULA=Y apt-get install -y msodbcsql18

RUN ACCEPT_EULA=Y apt-get install -y mssql-tools18 \
    && echo 'export PATH="$PATH:/opt/mssql-tools18/bin"' >> ~/.bashrc \
    && . ~/.bashrc

RUN apt-get install -y --no-install-recommends \
    unixodbc \
    unixodbc-dev \
    && rm -rf /var/lib/apt/lists/*

# Clone the GitHub repository into the container
RUN git clone https://github.com/Topi-Batu/ml-deployment /app

# Install any needed packages specified in requirements.txt
RUN pip install --no-cache-dir -r requirements.txt

# Expose a port if your application requires it
EXPOSE 80

# Run your application
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "80"]