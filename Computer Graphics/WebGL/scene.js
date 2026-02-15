  import * as entityBuilder from "./entityBuilder.js"; // needed for adding entities to the scene

  // create scene and skybox
  const scene = new THREE.Scene();
  const textureLoader = new THREE.TextureLoader();
  const skyTexture = textureLoader.load("Fuzzy_Sky-Blue_01-1024x512.png");
  skyTexture.mapping = THREE.EquirectangularReflectionMapping;
  scene.background = skyTexture;

  // create camera and set position
  const camera = new THREE.PerspectiveCamera(
    75,
    window.innerWidth / window.innerHeight,
    0.1,
    3000,
  );
  camera.position.set(0, 5, -10);

  // create renderer
  const renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
  renderer.shadowMap.enabled = true;
  renderer.shadowMap.type = THREE.PCFSoftShadowMap;
  renderer.setSize(window.innerWidth, window.innerHeight);
  document.body.appendChild(renderer.domElement);

  const controls = new THREE.OrbitControls(camera, renderer.domElement);
  controls.maxPolarAngle = Math.PI / 2 - 0.05;

  // add lighting and sun
  const ambientLight = new THREE.AmbientLight(0xffffff, 0.4);
  scene.add(ambientLight);

  const orbitRadius = 15;
  const orbitHeight = 8;
  const orbitSpeed = 1;

  let { sun, light } = entityBuilder.buildSun(
    orbitRadius,
    orbitHeight,
    orbitRadius,
  );
  scene.add(sun, light);

  let { grass, ground } = entityBuilder.buildGround();
  scene.add(grass, ground);

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
  const monument = entityBuilder.buildMonument();
  monument.position.set(0, 1, 0);
  scene.add(monument);

  const street = entityBuilder.buildStreet(10);
  scene.add(street);

  function render() {
    requestAnimationFrame(render);

    const time = Date.now() * 0.001 * orbitSpeed;

    const x = Math.cos(time) * orbitRadius;
    const y = orbitHeight;
    const z = Math.sin(time) * orbitRadius;

    sun.position.set(x, y, z);
    light.position.set(x, y, z);

    if (camera.position.y < 0.2) camera.position.y = 0.2;

    if (monument.userData.update) {
      monument.userData.update(time);
    }

    controls.update();
    renderer.render(scene, camera);
  }

  render();
