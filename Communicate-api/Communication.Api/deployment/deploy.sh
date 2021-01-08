echo "Proceeding with deployment..."
DOCKER_LOGIN=`aws ecr get-login --no-include-email --region ap-southeast-1`
sudo ${DOCKER_LOGIN}
if [[ -z "${DOCKER_OVERRIDE}" ]]; then
sudo -E -u go bash << EOF  
	echo down container
    docker-compose -f docker-compose-dev.yml down
	docker rmi -f $(sudo docker images -q ${DOCKER_REGISTRY_URL}/${DOCKER_IMAGE_REPO})
	echo pull images
	docker-compose -f docker-compose-dev.yml pull
	docker-compose -f docker-compose-dev.yml up -d
EOF
else
bash << EOF
    echo down container
    docker-compose -f docker-compose-old.yml -f ${DOCKER_OVERRIDE}-override.yml down
    docker rmi -f $(docker images -q ${DOCKER_REGISTRY_URL}/${DOCKER_IMAGE_REPO})
    echo pull images
    docker-compose -f docker-compose-old.yml -f ${DOCKER_OVERRIDE}-override.yml pull
    echo run docker DOCKER_OVERRIDE:${DOCKER_OVERRIDE}
    docker-compose -f docker-compose-old.yml -f ${DOCKER_OVERRIDE}-override.yml up -d
EOF
fi
