build-image:
	docker build -t reactivities-backend .

test-run:
	 docker run -it --rm -p 8000:3000 --name reactivities-bakend-test reactivities-backend