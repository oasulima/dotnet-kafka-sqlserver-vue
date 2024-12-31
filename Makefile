stop-all:
	docker compose --profile all down
run-env:
	docker compose --profile env up -d
build-services:
	docker compose --profile services build
run-services:
	docker compose --profile services up -d
stop-services:
	docker compose --profile services down
run-emulator:
	docker compose --profile emulator up -d
build-emulator:
	docker compose --profile emulator build
stop-emulator:
	docker compose --profile emulator down
prune:
	docker volume prune -a