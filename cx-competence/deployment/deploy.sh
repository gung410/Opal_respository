echo "Proceeding with deployment..."
DOCKER_LOGIN=`aws ecr get-login --no-include-email --region ap-southeast-1`
sudo ${DOCKER_LOGIN}
sudo -E -u go bash << EOF  
	echo down container
    sudo -E docker-compose down
	sudo docker rmi -f $(sudo docker images -q ${DOCKER_REGISTRY_URL}/${DOCKER_IMAGE_REPO})
	echo pull images
	sudo -E docker-compose pull
	if [[ -z "${USE_CLUSTER}" ]]; then
	echo run docker USE_CLUSTER:false
	sudo -E docker-compose up -d
	else
	echo run docker USE_CLUSTER:true
	sudo -E docker-compose -f docker-compose.yml -f ${ENVIRONMENT_NAME}-override.yml up -d
	fi
EOF
