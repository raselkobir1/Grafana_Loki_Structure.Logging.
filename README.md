# GrafanaLoki instruction

i. Graphana : Log visualization tools.
ii) Laki :Horzontal scheling data source. When error occur we push Laki server using serilog. After that it showing graphana.
iii) Pull docker image for Laki and graphana.
iv) Create a docker compose file and write command for create Graphana and Laki container. 
v) We find log by browing url http://localhost:3000 default userId: admin and password: admin
vi) Add Data source Laki and configure url http://laki:3100 and set credential.
vii) Then click Explore tab and browing log if any. 
viii) Global error handling middleware.
ix) Serilog setup.
x) Utilities common return methods
xi) elasticsearch, kibana

#Resource flow for elasticsearch kibana:
i) https://www.humankode.com/asp-net-core/logging-with-elasticsearch-kibana-asp-net-core-and-docker/
ii) https://www.youtube.com/watch?v=zp6A5QCW_II
