# syntax=docker/dockerfile:1

# ---- Build stage -----------------------------------------------------
FROM node:24-alpine AS build
WORKDIR /app

# Inject host CA certificates to fix TLS interception (UNABLE_TO_VERIFY_LEAF_SIGNATURE).
# Place your corporate/proxy CA as certs/win-cas.crt at repo root (gitignored).
COPY certs/ /tmp/certs/
RUN apk add --no-cache ca-certificates && \
    if ls /tmp/certs/*.crt > /dev/null 2>&1; then \
      for f in /tmp/certs/*.crt; do cp "$f" /usr/local/share/ca-certificates/; done && \
      update-ca-certificates; \
    fi

COPY EcoCharge.Client/package.json EcoCharge.Client/package-lock.json ./
# NODE_OPTIONS=--use-system-ca makes Node.js use the OS trust store (incl. our injected CA).
RUN NODE_OPTIONS=--use-system-ca npm ci

COPY EcoCharge.Client/ .
# VITE_API_BASE_URL can be overridden at build time, e.g.:
#   docker build --build-arg VITE_API_BASE_URL=https://api.example.com .
ARG VITE_API_BASE_URL=/api
ENV VITE_API_BASE_URL=${VITE_API_BASE_URL}
RUN npm run build

# ---- Runtime stage -----------------------------------------------------
FROM nginx:1.27-alpine AS runtime
COPY --from=build /app/dist /usr/share/nginx/html
COPY EcoCharge.Client/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
