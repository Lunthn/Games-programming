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
camera.position.set(0, 5, -10);

const renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
renderer.shadowMap.enabled = true;
renderer.shadowMap.type = THREE.PCFSoftShadowMap;
renderer.setSize(window.innerWidth, window.innerHeight);
document.body.appendChild(renderer.domElement);

const controls = new THREE.OrbitControls(camera, renderer.domElement);
controls.maxPolarAngle = Math.PI / 2 - 0.05;

const ambientLight = new THREE.AmbientLight(0xffffff, 0.4);
scene.add(ambientLight);

let { sun, light } = entityBuilder.buildSun();
scene.add(sun, light);

const ground = new THREE.Mesh(
  new THREE.PlaneGeometry(20, 20),
  new THREE.MeshStandardMaterial({ color: 0x6b8e23 }),
);
ground.rotation.x = -Math.PI / 2;
ground.receiveShadow = true;
scene.add(ground);

for (let i = -4; i < 5; i += 2) {
  const house1 = entityBuilder.buildHouse();
  house1.position.set(i, 0, -3);
  house1.rotation.y = Math.PI;
  const house2 = entityBuilder.buildHouse();
  house2.position.set(i, 0, 3);
  const tree1 = entityBuilder.buildTree();
  tree1.position.set(
    i + 1 + (Math.random() - 0.5) * 0.5,
    0,
    -1.5 + (Math.random() - 0.5) * 1.5,
  );
  const tree2 = entityBuilder.buildTree();
  tree2.position.set(
    i - 1 + (Math.random() - 0.5) * 0.5,
    0,
    1.5 + (Math.random() - 0.5) * 1.5,
  );
  const hydrant = entityBuilder.buildFireHydrant();
  hydrant.position.set(i + 0.8, 0.05, -0.8);
  scene.add(house1, house2, tree1, tree2, hydrant);
}

const street = entityBuilder.buildStreet(10);
scene.add(street);

const orbitRadius = 15;
const orbitHeight = 10;
const orbitSpeed = 0.5;

function render() {
  requestAnimationFrame(render);

  const time = Date.now() * 0.001 * orbitSpeed;

  const x = Math.cos(time) * orbitRadius;
  const z = Math.sin(time) * orbitRadius;
  const y = orbitHeight;

  sun.position.set(x, y, z);
  light.position.set(x, y, z);

  light.intensity = 0.5;

  if (camera.position.y < 0.2) camera.position.y = 0.2;

  controls.update();
  renderer.render(scene, camera);
}

render();
