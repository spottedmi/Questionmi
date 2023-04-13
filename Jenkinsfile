pipeline{
	agent any
	
	environment {
		
		// TAG_NAME = 'latest';
		REPO_USER = "${scm.getUserRemoteConfigs()[0].getUrl().tokenize('/')[-2].toLowerCase()}";
		REPO_NAME = "${scm.getUserRemoteConfigs()[0].getUrl().tokenize('/').last().split("\\.")[0].toLowerCase()}";
		
		// REPO = "$REPO_USER/$REPO_NAME";
		REPO = "randomguy090/$REPO_NAME";
		RUN_FOR = "main,develop";
		
		HEARTBEAT_CHECK_INTERVAL=300;
	}

	stages{
		stage("Preparing"){
			steps{


				script{

					if (env.BRANCH_NAME == "main"){

						sh 'echo latest > TAG_NAME';
					}else if (env.BRANCH_NAME == "develop"){
						sh 'echo develop > TAG_NAME';

					}else{
						sh "echo ${env.BRANCH_NAME} > TAG_NAME";

					}
					
					 TAG_NAME = readFile('TAG_NAME').trim()					


					if( ! env.RUN_FOR.tokenize(",").contains(env.BRANCH_NAME) ) {
						currentBuild.result = 'SUCCESS';
						return
					}
					

					sh "apt update && apt upgrade -y ";
					sh "apt install curl -y ";
					
					sh "apt install docker -y ";

					echo "build tag ${env.BUILD_TAG}";
					echo "repo name: ${scm.getUserRemoteConfigs()[0].getUrl()}"
				}
			}
		}
		
		stage("Building-Docker"){
			steps{
				script {

					echo "---------------building---------------";
					echo "building docker image via built in function";
					IMG = docker.build("$REPO:$TAG_NAME");
					echo "build image: $IMG";

				}
			}
		}

		stage("release"){
			steps{	
				script {
					withCredentials([usernamePassword(credentialsId: "github_token", passwordVariable: 'githubSecret', usernameVariable: 'githubUser')]) {
							sh "cp dist/run run.exe"
							sh "curl https://raw.githubusercontent.com/RandomGuy090/github-auto-release/main/auto-release.sh > run.sh";
							sh "bash run.sh -u spottedmi -r TelloBeep -t $githubSecret -b $TAG_NAME -e run.exe  > VERSION"
							VERSION = readFile('VERSION').trim()
							echo VERSION;					
							echo VERSION;					

						}
					

				}
			}
		}
				stage('Docker Push') {
			      steps {
				      script{
						echo "---------------pushing to docker hub---------------";

					      withCredentials([usernamePassword(credentialsId: "docker_token", passwordVariable: 'dockerHubPassword', usernameVariable: 'dockerHubUser')]) {
							echo "deploying: $IMG:latest";
							IMG.push(TAG_NAME);
							// sh "docker rmi $IMG.id"
						      
						}
						withCredentials([usernamePassword(credentialsId: "docker_token", passwordVariable: 'dockerHubPassword', usernameVariable: 'dockerHubUser')]) {
							echo "deploying: $IMG:$VERSION";
							IMG.push(VERSION);
							sh "docker rmi $IMG.id"
						      
						}
					}
			    } 
		}

		stage("deploy"){
			steps{	
				script {
					echo "---------------deploying---------------";

				}
			}
		}
		
	}
}
