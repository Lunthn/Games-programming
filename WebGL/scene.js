import * as entityBuilder from "./entityBuilder.js";

const scene = new THREE.Scene();
const textureLoader = new THREE.TextureLoader();
const skyTexture = textureLoader.load("Fuzzy_Sky-Blue_01-1024x512.png");
skyTexture.mapping = THREE.EquirectangularReflectionMapping;
scene.background = skyTexture;
const camera = new THREE.PerspectiveCamera(
  75,
  window.innerWidth / window.innerHeight,
  0.1,
  3000,
);
camera.position.set(0, 2, -2);

const clock = new THREE.Clock();

const renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
renderer.shadowMap.enabled = true;
renderer.shadowMap.type = THREE.PCFSoftShadowMap;

const controls = new THREE.OrbitControls(camera, renderer.domElement);
controls.maxPolarAngle = Math.PI / 2 - 0.05;

renderer.setSize(window.innerWidth, window.innerHeight);
document.body.appendChild(renderer.domElement);

const ambientLight = new THREE.AmbientLight(0xffffff, 0.5);
scene.add(ambientLight);

const directionalLight = new THREE.DirectionalLight(0xffffff, 1);
directionalLight.position.set(100, 100, -50);
directionalLight.castShadow = true;
directionalLight.shadow.mapSize.width = 1024;
directionalLight.shadow.mapSize.height = 1024;

scene.add(directionalLight);

const groundGeometry = new THREE.PlaneGeometry(200, 200);
const groundMaterial = new THREE.MeshStandardMaterial({ color: 0x6b8e23 });
const ground = new THREE.Mesh(groundGeometry, groundMaterial);
ground.rotation.x = -Math.PI / 2;
ground.position.y = 0;
ground.receiveShadow = true;
scene.add(ground);

for (let i = -3; i < 4; i += 2) {
  let house = entityBuilder.buildHouse();
  house.position.set(i, 0, -3);
  house.rotation.y = Math.PI;
  scene.add(house);

  let house2 = entityBuilder.buildHouse();
  house2.position.set(i, 0, 3);
  scene.add(house2);

  let tree = entityBuilder.buildTree();
  let x = 1 + (Math.random() - 0.5) * 0.5;
  let y = (Math.random() - 0.5) * 1.5;

  tree.position.set(i + x, 0, -1.5 + y);
  scene.add(tree);

  let tree2 = entityBuilder.buildTree();
  let x2 = -1 + (Math.random() - 0.5) * 0.5;
  let y2 = (Math.random() - 0.5) * 1.5;

  tree2.position.set(i + x, 0, 1.5 + y);
  scene.add(tree2);

  let fireHydrant = entityBuilder.buildFireHydrant();
  fireHydrant.position.set(i + 0.8, 0.05, -0.8);
  scene.add(fireHydrant);
}

let street = entityBuilder.buildStreet(9);
street.position.set(0, 0, 0);
scene.add(street);

const render = function () {
  requestAnimationFrame(render);
  controls.update();

  if (camera.position.y < 0.2) {
    camera.position.y = 0.2;
  }
  renderer.render(scene, camera);
};

render();
