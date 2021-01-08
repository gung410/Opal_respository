echo "Proceeding with deployment..."
DOCKER_LOGIN=`aws ecr get-login --no-include-email --region ap-southeast-1`
sudo ${DOCKER_LOGIN}
sudo -E -u go bash << EOF  
	echo down container
    sudo -E docker-compose -f docker-compose-dev.yml down
	sudo docker rmi -f $(sudo docker images -q ${DOCKER_REGISTRY_URL}/${DOCKER_IMAGE_REPO})
	echo pull images
	sudo -E docker-compose -f docker-compose-dev.yml pull
	sudo -E docker-compose -f docker-compose-dev.yml up -d
EOF
